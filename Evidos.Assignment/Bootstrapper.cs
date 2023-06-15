using Evidos.Assignment.Messaging.Commands;
using Evidos.Assignment.Messaging.Events;
using Evidos.Assignment.Messaging.Queries;
using Evidos.Assignment.Persistence;
using Evidos.Assignment.Users;
using Evidos.Assignment.Users.Features;
using Evidos.Assignment.Users.Repositories;
using Microsoft.Extensions.DependencyInjection;


namespace Evidos.Assignment
{
    public static class Bootstrapper
    {
        public static void BootstrapServices(this IServiceCollection services)
        {
            // Register Bus
            services.AddSingleton<IEventBus, EventBus>();
            services.AddSingleton<IQueryBus, QueryBus>();
            services.AddSingleton<ICommandBus, CommandBus>();

            // Register persistence
            services.AddSingleton<IUserProjectionRepository, InMemoryUserProjectionRepository>();
            services.AddSingleton<IUserAggregateRepository, InMemoryUserAggregateRepository>();
            services.AddSingleton<IEventStore, InMemoryEventStore>();
            services.AddSingleton<IUserPrivateDataRepository, InMemoryUserPrivateDataRepository>();
            services.AddSingleton<IPendingEmailChangeRepository, InMemoryPendingEmailChangeRepository>();

            // Register commands handlers
            services.AddTransient<ICommandHandler<CreateUserCommand>, CreateUserCommandHandler>();
            services.AddTransient<ICommandHandler<ConfirmEmailChange>, ConfirmEmailChangeCommandHandler>();
            services.AddTransient<ICommandHandler<DeleteUserCommand>, DeleteUserCommandHandler>();
            services.AddTransient<ICommandHandler<RequestEmailChangeCommand>, RequestEmailChangeCommandHandler>();

            // Register query handlers
            services.AddTransient<IQueryHandler<GetUserByEmailQuery, UserDto>, GetUserByEmailQueryHandler>();
            services.AddTransient<IQueryHandler<SearchUsersByFullOrPartialEmailQuery, IEnumerable<UserDto>>, SearchUsersByFullOrPartialEmailQueryHandler>();
            services.AddTransient<IQueryHandler<GetPendingEmailRequestsQuery, IEnumerable<PendingEmailChangeDto>>, GetPendingEmailRequestsQueryHandler>();

            // Register events handlers
            services.AddTransient<IEventHandler<UserCreated>, UserEventLifecycle>();
            services.AddTransient<IEventHandler<UserDeleted>, UserEventLifecycle>();
            services.AddTransient<IEventHandler<EmailChangeRequested>, UserEventLifecycle>();
            services.AddTransient<IEventHandler<UserEmailUpdated>, UserEventLifecycle>();

        }
    }
}
