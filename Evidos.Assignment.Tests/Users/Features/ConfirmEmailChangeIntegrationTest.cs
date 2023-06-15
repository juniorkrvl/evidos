using Evidos.Assignment.Messaging.Commands;
using Evidos.Assignment.Tests.Fixtures;
using Evidos.Assignment.Users;
using Evidos.Assignment.Users.Features;
using Evidos.Assignment.Users.Models;
using Evidos.Assignment.Users.Repositories;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Evidos.Assignment.Tests.Users.Features;

[TestFixture]
public class ConfirmEmailChangeIntegrationTest
{
    private IUserPrivateDataRepository _userPrivateDataRepository;
    private IPendingEmailChangeRepository _emailChangeRepository;
    private ICommandBus _commandBus;
    private DummyEventBus _dummyEventBus;

    [SetUp]
    public void Init()
    {
        _dummyEventBus = new DummyEventBus();
        var serviceProvider = IntegrationTestsFixtures.SetupServiceProvider(_dummyEventBus);
        
        _emailChangeRepository = serviceProvider.GetRequiredService<IPendingEmailChangeRepository>(); 
        _commandBus = serviceProvider.GetRequiredService<ICommandBus>();
        _userPrivateDataRepository = serviceProvider.GetRequiredService<IUserPrivateDataRepository>();
    }

    [Test]
    public async Task ShouldUpdateEmailAndRemovePendingRequest()
    {
        // Arrange
        
        // Save a user
        var userId = UserId.Generate();
        await _userPrivateDataRepository.Save(
            new UserData(
                userId,
                "Junior",
                Email.FromString("junior@test.com"),
                "Amsterdam"
            )
        );
        
        // Save request token
        var emailChangeToken = EmailChangeToken.Generate();
        await _emailChangeRepository.Save(
            new PendingEmailChange(
                emailChangeToken, 
                userId,
                Email.FromString("junior@example.nl")
            )
        );

        var command = new ConfirmEmailChange(emailChangeToken);
        
        // Act

        await _commandBus.DispatchAsync(command);

        // Assert
        
        // Pending request was deleted
        var pendingRequest = await _emailChangeRepository.GetByToken(emailChangeToken);
        Assert.IsNull(pendingRequest);
        
        // User email got updated
        var user = await _userPrivateDataRepository.GetByUserId(userId);
        Assert.NotNull(user);
        Assert.AreEqual(user!.Email, Email.FromString("junior@example.nl"));
        
        // Event was dispatched
        var dispatchedEvent = _dummyEventBus.DispatchedEvents().Result.First();
        Assert.NotNull(dispatchedEvent);
        Assert.True(dispatchedEvent is UserEmailUpdated);
        Assert.AreEqual((dispatchedEvent as UserEmailUpdated)!.UserId, userId);
    }

    [Test]
    public async Task ShouldNotUpdateEmailIfThereIsNoPendingRequest()
    {
        // Arrange
        var emailChangeToken = EmailChangeToken.Generate();
        var command = new ConfirmEmailChange(emailChangeToken);

        // Act
        var exception = Assert.ThrowsAsync<CannotChangeEmail>(() => _commandBus.DispatchAsync(command));

        // Assert
        Assert.NotNull(exception);
        Assert.AreEqual(
            $"There is no pending email request for the provided token {emailChangeToken}",
            exception!.Message
        );
    }
    
    [Test]
    public async Task ShouldNotUpdateEmailIfUserNotFound()
    {
        // Arrange
        var userId = UserId.Generate();
        var emailChangeToken = EmailChangeToken.Generate();
        
        await _emailChangeRepository.Save(
            new PendingEmailChange(
                emailChangeToken, 
                userId,
                Email.FromString("junior@example.nl")
            )
        );
        var command = new ConfirmEmailChange(emailChangeToken);

        // Act
        var exception = Assert.ThrowsAsync<CannotRetrieveUser>(() => _commandBus.DispatchAsync(command));

        // Assert
        Assert.NotNull(exception);
        Assert.AreEqual(
            $"The private data for user {userId.ToString()} is not available.",
            exception!.Message
        );
        
        // Assert no event dispatched
        Assert.True(!_dummyEventBus.DispatchedEvents().Result.Any());
    }
}