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
public class CreateUserIntegrationTest
{
    private IUserPrivateDataRepository _userPrivateDataRepository;
    private ICommandBus _commandBus;
    private DummyEventBus _dummyEventBus;

    [SetUp]
    public void Init()
    {
        _dummyEventBus = new DummyEventBus();
        var serviceProvider = IntegrationTestsFixtures.SetupServiceProvider(_dummyEventBus);
        
        _commandBus = serviceProvider.GetRequiredService<ICommandBus>();
        _userPrivateDataRepository = serviceProvider.GetRequiredService<IUserPrivateDataRepository>();
    }

    [Test]
    public async Task ShouldCreateNewUserAndProjection()
    {
        // Arrange
        var command = new CreateUserCommand("Junior", Email.FromString("junior@test.com"), "Amsterdam");
        
        // Act
        await _commandBus.DispatchAsync(command);
        
        // Assert event UserCreated dispatched
        var dispatchedEvent = _dummyEventBus.DispatchedEvents().Result.First();
        Assert.True(dispatchedEvent is UserCreated);
        
        // Assert Private data saved
        var privateData = await _userPrivateDataRepository.GetByUserId((dispatchedEvent as UserCreated)!.UserId);
        Assert.AreEqual(privateData!.UserId, (dispatchedEvent as UserCreated)!.UserId);
        Assert.AreEqual(privateData!.Name, command.Name);
        Assert.AreEqual(privateData!.Email, command.Email);
        Assert.AreEqual(privateData!.Address, command.Address);
    }

    [Test]
    public async Task ShouldNotCreateAUserForAnExistingEmail()
    {
        // Arrange
        var command = new CreateUserCommand("Junior", Email.FromString("junior@test.com"), "Amsterdam");
        await _commandBus.DispatchAsync(command);
        
        // Act
        var exception = Assert.ThrowsAsync<CannotCreateUser>(() => _commandBus.DispatchAsync(command));

        // Assert
        Assert.NotNull(exception);
        Assert.AreEqual(
            $"Cannot create user because email {command.Email.ToString()} already exists in the system",
            exception!.Message
        );
        
        // Assert second event not dispatched
        Assert.True(_dummyEventBus.DispatchedEvents().Result.Count() < 2);
    }


}