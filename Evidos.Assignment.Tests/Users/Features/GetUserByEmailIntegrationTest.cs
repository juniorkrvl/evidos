using Evidos.Assignment.Messaging.Commands;
using Evidos.Assignment.Messaging.Queries;
using Evidos.Assignment.Tests.Fixtures;
using Evidos.Assignment.Users;
using Evidos.Assignment.Users.Features;
using Evidos.Assignment.Users.Models;
using Evidos.Assignment.Users.Repositories;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Evidos.Assignment.Tests.Users.Features;

[TestFixture]
public class GetUserByEmailIntegrationTest
{
    private IUserProjectionRepository _userProjectionRepository;
    private IQueryBus _queryBus;

    [SetUp]
    public void Init()
    {
        var serviceProvider = IntegrationTestsFixtures.SetupServiceProvider();
        _queryBus = serviceProvider.GetRequiredService<IQueryBus>();
        _userProjectionRepository = serviceProvider.GetRequiredService<IUserProjectionRepository>();
    }

    [Test]
    public async Task ShouldReturnExistingUserForGivenEmail()
    {
        // Arrange
        
        // Create new user
        var userId = UserId.Generate();
        await _userProjectionRepository.Save(
            new UserViewModel(userId, "Junior", Email.FromString("junior@test.com"), "Amsterdam")
        );

        var query = new GetUserByEmailQuery(Email.FromString("junior@test.com"));

        // Act
        var result = await _queryBus.DispatchAsync<GetUserByEmailQuery, UserDto>(query);

        // Assert
        Assert.AreEqual(result.Id, userId);
        Assert.AreEqual(result.Email, Email.FromString("junior@test.com"));
        Assert.AreEqual(result.Name, "Junior");
        Assert.AreEqual(result.Address, "Amsterdam");
        Assert.AreEqual(result.Id, userId);
    }
    
    [Test]
    public async Task ShouldThrowUserNotFound()
    {
        // Arrange
        var query = new GetUserByEmailQuery(Email.FromString("junior@test.com"));

        // Act
        var exception = Assert.ThrowsAsync<CannotRetrieveUser>(()=> _queryBus.DispatchAsync<GetUserByEmailQuery, UserDto>(query));

        // Assert
        Assert.IsNotNull(exception);
        Assert.AreEqual(
            $"The user with email junior@test.com was not found",
            exception!.Message
        );
    }
}