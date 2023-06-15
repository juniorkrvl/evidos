using Evidos.Assignment.Messaging.Commands;
using Evidos.Assignment.Messaging.Events;
using Evidos.Assignment.Tests.Fixtures;
using Evidos.Assignment.Users;
using Evidos.Assignment.Users.Features;
using Evidos.Assignment.Users.Models;
using Evidos.Assignment.Users.Repositories;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Evidos.Assignment.Tests.Users.Features;

[TestFixture]
public class RequestEmailChangeIntegrationTest
{
    private IPendingEmailChangeRepository _emailChangeRepository;
    private IUserPrivateDataRepository _userPrivateDataRepository;
    private ICommandBus _commandBus;
    private DummyEventBus _dummyEventBus;

    [SetUp]
    public void Init()
    {
        _dummyEventBus = new DummyEventBus();
        var serviceProvider = IntegrationTestsFixtures.SetupServiceProvider(_dummyEventBus);
        _commandBus = serviceProvider.GetRequiredService<ICommandBus>();
        _emailChangeRepository = serviceProvider.GetRequiredService<IPendingEmailChangeRepository>();
        _userPrivateDataRepository =serviceProvider.GetRequiredService<IUserPrivateDataRepository>();
    }

    [Test]
    public async Task ShouldCreatePendingEmailChangeRequest()
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
        
        // Request an email change for the user
        var requestEmailChangeCommand = new RequestEmailChangeCommand(userId, Email.FromString("junior@test.nl"));
        
        // Act
        await _commandBus.DispatchAsync(requestEmailChangeCommand);
        
        // Assert Change Request was stored
        var requests = await _emailChangeRepository.GetAll();
        var request = requests.First();
        
        Assert.NotNull(request);
        Assert.AreEqual(request.NewEmail, requestEmailChangeCommand.NewEmail);
        Assert.AreEqual(request.UserId, requestEmailChangeCommand.UserId);
        
        // Assert event was dispatched
        var dispatchedEvent = _dummyEventBus.DispatchedEvents().Result.First();
        Assert.True(dispatchedEvent is EmailChangeRequested);
        Assert.AreEqual(((dispatchedEvent as EmailChangeRequested)!).UserId, request.UserId);
        Assert.AreEqual(((dispatchedEvent as EmailChangeRequested)!).Token, request.Token);
    }

    [Test]
    public async Task ShouldNotAllowRequestChangeToTheSameEmail()
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
        
        // Request an email change for the user
        var requestEmailChangeCommand = new RequestEmailChangeCommand(userId, Email.FromString("junior@test.com"));
        
        // Act
        var exception = Assert.ThrowsAsync<CannotChangeEmail>(()=>_commandBus.DispatchAsync(requestEmailChangeCommand));
        
        // Assert
        Assert.NotNull(exception);
        Assert.AreEqual(
            $"The email you are trying to change to ({requestEmailChangeCommand.NewEmail.ToString()}) is already the user current's email",
            exception!.Message
        );
        
        // Assert no event dispatched
        Assert.True(!_dummyEventBus.DispatchedEvents().Result.Any());

    }
    
    [Test]
    public Task ShouldNotAllowRequestChangeWhenUserNotFound()
    {
        // Arrange
        
        // Save a user
        var userId = UserId.Generate();
        
        // Request an email change for the user
        var requestEmailChangeCommand = new RequestEmailChangeCommand(userId, Email.FromString("junior@test.com"));
        
        // Act
        var exception = Assert.ThrowsAsync<CannotRetrieveUser>(()=>_commandBus.DispatchAsync(requestEmailChangeCommand));
        
        // Assert
        Assert.NotNull(exception);
        Assert.AreEqual(
            $"The user with id {userId.ToString()} was not found",
            exception!.Message
        );
        
        // Assert no event dispatched
        Assert.True(!_dummyEventBus.DispatchedEvents().Result.Any());
        
        return Task.CompletedTask;
    }
    
}