using Evidos.Assignment.Messaging.Commands;
using Evidos.Assignment.Messaging.Events;
using Evidos.Assignment.Tests.Fixtures;
using Evidos.Assignment.Users.Features;
using Evidos.Assignment.Users.Models;
using Evidos.Assignment.Users.Repositories;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Evidos.Assignment.Tests.Users.Events;

[TestFixture]
public class UserEventsLifecycleIntegrationTest
{
    private IUserPrivateDataRepository _userPrivateDataRepository;
    private IUserProjectionRepository _projectionRepository;
    private IEventBus _eventBus;

    [SetUp]
    public void Init()
    {
        var serviceProvider = IntegrationTestsFixtures.SetupServiceProvider();
        
        _eventBus = serviceProvider.GetRequiredService<IEventBus>();
        _projectionRepository = serviceProvider.GetRequiredService<IUserProjectionRepository>();
        _userPrivateDataRepository = serviceProvider.GetRequiredService<IUserPrivateDataRepository>();
    }

    [Test]
    public async Task ShouldCreateUserProjectionWhenUserCreatedEventDispatched()
    {
        // Arrange
        
        // Save user private data
        var userId = UserId.Generate();
        var userData = new UserData(userId, "Junior", Email.FromString("junior@test.com"), "Amsterdam");
        await _userPrivateDataRepository.Save(userData);
        
        var @event = new UserCreated(userId, DateTime.UtcNow);
        
        // Act
        await _eventBus.Publish(@event);
        
        // Assert
        var userProjection = await _projectionRepository.GetById(userId);
        Assert.NotNull(userProjection);
        Assert.AreEqual(userProjection!.Id, userData.UserId);
        Assert.AreEqual(userProjection!.Name, userData.Name);
        Assert.AreEqual(userProjection!.Email, userData.Email);
        Assert.AreEqual(userProjection!.Address, userData.Address);
    }
    
    [Test]
    public async Task ShouldUpdateUserEmailWhenUserEmailUpdatedEventDispatched()
    {
        // Arrange
        
        // Save user private data
        var userId = UserId.Generate();
        var userData = new UserData(userId, "Junior", Email.FromString("jnr@email.com"), "Amsterdam");
        await _userPrivateDataRepository.Save(userData);
        
        // Setup projection with email outdated
        await _projectionRepository.Save(
            new UserViewModel(userId, "Junior", Email.FromString("jnr@email.com"), "Amsterdam")
        );
        
        // Update email in the private repository
        userData.UpdateEmail(Email.FromString("junior@email.com"));
        
        // Dispatch event
        var @event = new UserEmailUpdated(userId, DateTime.UtcNow);
        
        // Act
        await _eventBus.Publish(@event);
        
        // Assert
        var userProjection = await _projectionRepository.GetById(userId);
        Assert.NotNull(userProjection);
        Assert.AreEqual(userProjection!.Id, userData.UserId);
        Assert.AreEqual(userProjection!.Name, userData.Name);
        Assert.AreEqual(userProjection!.Email, Email.FromString("junior@email.com"));
        Assert.AreEqual(userProjection!.Address, userData.Address);
    }

    [Test]
    public async Task ShouldDeleteAllUserDataWhenUserDeletedEventDispatched()
    {
        // Arrange
        // Setup projection 
        
        var userId = UserId.Generate();
        await _projectionRepository.Save(
            new UserViewModel(userId, "Junior", Email.FromString("jnr@email.com"), "Amsterdam")
        );

        var @event = new UserDeleted(userId, DateTime.UtcNow);

        // Act
        await _eventBus.Publish(@event);

        // Assert
        var user = await _projectionRepository.GetById(userId);
        Assert.IsNull(user);
    }
    
}