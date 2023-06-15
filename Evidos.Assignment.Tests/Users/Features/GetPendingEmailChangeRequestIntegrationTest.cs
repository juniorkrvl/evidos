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
public class GetPendingEmailChangeRequestIntegrationTest
{
    private IPendingEmailChangeRepository _pendingEmailChangeRepository;
    private IQueryBus _queryBus;
    
    
    [SetUp]
    public void Init()
    {
        var serviceProvider = IntegrationTestsFixtures.SetupServiceProvider();
        _pendingEmailChangeRepository = serviceProvider.GetRequiredService<IPendingEmailChangeRepository>();    
        _queryBus = serviceProvider.GetRequiredService<IQueryBus>();    
    }

    [Test]
    public async Task ShouldReturnListOfPendingEmailChangesGivenUserId()
    {
        // Arrange
        var userId = UserId.Generate();
        var token = EmailChangeToken.Generate();
        
        // Create pending request
        var pendingChange = new PendingEmailChange(token, userId, Email.FromString("junior@test.com"));
        await _pendingEmailChangeRepository.Save(pendingChange);

        var query = new GetPendingEmailRequestsQuery();
        
        // Act
        var pendingChanges = await _queryBus.DispatchAsync<GetPendingEmailRequestsQuery, IEnumerable<PendingEmailChangeDto>>(query);
        
        // Assert
        var result = pendingChanges.First();
        Assert.AreEqual(result.Token, pendingChange.Token);
        Assert.AreEqual(result.NewEmail, pendingChange.NewEmail);
        Assert.AreEqual(result.UserId, pendingChange.UserId);
    }
    
    [Test]
    public async Task ShouldReturnEmptyListOfPendingEmailChangesGivenUserId()
    {
        // Arrange
        var query = new GetPendingEmailRequestsQuery();
        
        // Act
        var pendingChanges = await _queryBus.DispatchAsync<GetPendingEmailRequestsQuery, IEnumerable<PendingEmailChangeDto>>(query);
        
        // Assert
        Assert.True(!pendingChanges.Any());
    }
}