using Evidos.Assignment.Messaging.Queries;
using Evidos.Assignment.Tests.Fixtures;
using Evidos.Assignment.Users;
using Evidos.Assignment.Users.Features;
using Evidos.Assignment.Users.Models;
using Evidos.Assignment.Users.Repositories;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Evidos.Assignment.Tests.Users.Features;

public class SearchUserByEmailIntegrationTest
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
    public async Task ShouldReturnListWithFoundEmails()
    {
        // Arrange
        
        // Create new users
        var user1 = await CreateNewUser(Email.FromString("junior@test.com"));
        var user2 = await CreateNewUser(Email.FromString("junior@test.nl"));
        var user3 = await CreateNewUser(Email.FromString("junior@example.com"));

        var query = new SearchUsersByFullOrPartialEmailQuery("junior");

        // Act
        var results = await _queryBus.DispatchAsync<SearchUsersByFullOrPartialEmailQuery, IEnumerable<UserDto>>(query);

        // Assert
        var users = results as UserDto[] ?? results.ToArray();
        Assert.True(users.Length == 3);
        
        var user1Found = users.First(x => x.Email.Equals(Email.FromString("junior@test.com")));
        Assert.NotNull(user1Found);
        Assert.AreEqual(user1Found.Id, user1);
        
        var user2Found = users.First(x => x.Email.Equals(Email.FromString("junior@test.nl")));
        Assert.NotNull(user2Found);
        Assert.AreEqual(user2Found.Id, user2);
        
        var user3Found = users.First(x => x.Email.Equals(Email.FromString("junior@example.com")));
        Assert.NotNull(user3Found);
        Assert.AreEqual(user3Found.Id, user3);
    }
    
    
    
    [Test]
    public async Task ShouldReturnEmptyUserList()
    {
        // Arrange
        var query = new SearchUsersByFullOrPartialEmailQuery("junior");

        // Act
        var results = await _queryBus.DispatchAsync<SearchUsersByFullOrPartialEmailQuery, IEnumerable<UserDto>>(query);
        
        // Assert
        Assert.IsEmpty(results);
    }

    private async Task<UserId> CreateNewUser(Email email)
    {
        var userId = UserId.Generate();
        await _userProjectionRepository.Save(
            new UserViewModel(userId, "Junior", email, "Amsterdam")
        );
        return userId;
    }
}