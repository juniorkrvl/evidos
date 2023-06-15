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
public class DeleteUserIntegrationTest
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
        
        _commandBus = serviceProvider.GetRequiredService<ICommandBus>();
        _userPrivateDataRepository = serviceProvider.GetRequiredService<IUserPrivateDataRepository>();
        _emailChangeRepository = serviceProvider.GetRequiredService<IPendingEmailChangeRepository>();
    }

    [Test]
    public async Task ShouldDeleteAllPrivateInformationFromTheUser()
    {
        // Arrange
        var userId = UserId.Generate();
        
        // Create new user
        await _userPrivateDataRepository.Save(
            new UserData(userId, "Junior", Email.FromString("jnr@email.com"), "Amsterdam")
        );
        
        // Create new email change request
        var changeToken = EmailChangeToken.Generate();
        await _emailChangeRepository.Save(
            new PendingEmailChange(changeToken, userId, Email.FromString("junir@test.com"))
        );

        var command = new DeleteUserCommand(userId);

        // Act
        await _commandBus.DispatchAsync(command);

        // Assert user data was deleted
        var user = await _userPrivateDataRepository.GetByUserId(userId);
        Assert.IsNull(user);
        
        // Assert pending request was deleted
        var request = await _emailChangeRepository.GetByToken(changeToken);
        Assert.IsNull(request);
        
        // Assert event was dispatched
        var dispatchedEvents = _dummyEventBus.DispatchedEvents().Result.First();
        Assert.IsNotNull(dispatchedEvents);
        Assert.True(dispatchedEvents is UserDeleted);
        Assert.AreEqual((dispatchedEvents as UserDeleted)!.UserId, userId);
    }

    [Test]
    public Task ShouldThrowErrorIfPrivateDataAlreadyDeleted()
    {
        // Arrange
    
        // Save a user
        var userId = UserId.Generate();
    
        // Request an email change for the user
        var requestEmailChangeCommand = new DeleteUserCommand(userId);
    
        // Act
        var exception = Assert.ThrowsAsync<CannotRetrieveUser>(()=>_commandBus.DispatchAsync(requestEmailChangeCommand));
    
        // Assert
        Assert.NotNull(exception);
        Assert.AreEqual(
            $"The private data for user {userId.ToString()} is not available.",
            exception!.Message
        );
        
        // Assert no event dispatched
        Assert.True(!_dummyEventBus.DispatchedEvents().Result.Any());
    
        return Task.CompletedTask;
    }
}