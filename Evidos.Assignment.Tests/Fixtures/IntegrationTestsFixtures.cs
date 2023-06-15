using Evidos.Assignment.Messaging.Commands;
using Evidos.Assignment.Messaging.Events;
using Evidos.Assignment.Messaging.Queries;
using Evidos.Assignment.Persistence;
using Evidos.Assignment.Tests.Users.Events;
using Evidos.Assignment.Users;
using Evidos.Assignment.Users.Features;
using Evidos.Assignment.Users.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Evidos.Assignment.Tests.Fixtures;

public static class IntegrationTestsFixtures
{
    internal static IServiceProvider SetupServiceProvider(IEventBus? eventBusInstance = null)
    {
        IServiceCollection services = new ServiceCollection();

        // Custom Event Bus
        if (eventBusInstance!= null)
        {
            services.AddSingleton<IEventBus>(eventBusInstance);
        }
        else
        {
            services.AddSingleton<IEventBus, EventBus>();    
        }

        // Bus and Repositories
        services.AddSingleton<ICommandBus, CommandBus>();
        services.AddSingleton<IQueryBus, QueryBus>();
        services.AddSingleton<IEventStore, InMemoryEventStore>();
        services.AddSingleton<IUserAggregateRepository, InMemoryUserAggregateRepository>();
        services.AddSingleton<IUserPrivateDataRepository, InMemoryUserPrivateDataRepository>();
        services.AddSingleton<IUserAggregateRepository, InMemoryUserAggregateRepository>();
        services.AddSingleton<IUserProjectionRepository, InMemoryUserProjectionRepository>();
        services.AddSingleton<IPendingEmailChangeRepository, InMemoryPendingEmailChangeRepository>();
        
        // Commands
        services.AddSingleton<ICommandHandler<CreateUserCommand>, CreateUserCommandHandler>();
        services.AddSingleton<ICommandHandler<RequestEmailChangeCommand>, RequestEmailChangeCommandHandler>();
        services.AddSingleton<ICommandHandler<ConfirmEmailChange>, ConfirmEmailChangeCommandHandler>();
        services.AddSingleton<ICommandHandler<DeleteUserCommand>, DeleteUserCommandHandler>();
        
        // Events
        services.AddSingleton<IEventHandler<UserCreated>, UserEventLifecycle>();
        services.AddSingleton<IEventHandler<UserEmailUpdated>, UserEventLifecycle>();
        services.AddSingleton<IEventHandler<UserDeleted>, UserEventLifecycle>();
        services.AddSingleton<IEventHandler<EmailChangeRequested>, UserEventLifecycle>();
        
        // Queries
        services.AddSingleton<IQueryHandler<GetUserByEmailQuery,UserDto>, GetUserByEmailQueryHandler>();
        services.AddSingleton<
            IQueryHandler<SearchUsersByFullOrPartialEmailQuery,IEnumerable<UserDto>>, 
            SearchUsersByFullOrPartialEmailQueryHandler
        >();
        services.AddSingleton<
            IQueryHandler<GetPendingEmailRequestsQuery, IEnumerable<PendingEmailChangeDto>>,
            GetPendingEmailRequestsQueryHandler
        >();
        
        services.AddSingleton<IEventHandler<UserCreated>, UserEventLifecycle>();
        

        return services.BuildServiceProvider();
    }
    
    internal static IServiceProvider SetupDummyEventBus(IServiceCollection services, DummyEventBus eventBus)
    {
        // Bus and Repositories
        services.AddSingleton<IEventBus>(eventBus);
        
        // Commands
        return services.BuildServiceProvider();
    }

}