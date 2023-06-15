using System.CommandLine;
using System.CommandLine.Invocation;
using Evidos.Assignment.Users.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text;
using Evidos.Assignment.Messaging.Events;

namespace Evidos.Assignment.ConsoleApp.CommandLineInterface;

internal class GetEventsCommand
{
    internal static Command Setup(ServiceProvider di)
    {
        var queryOption = new Option<string>(new string[] { "--user-id", "-id" }, description: "User id")
        {
            Required = true
        };
            
        var command = new Command("events", "Retrieve stream of events per user")
        {
            queryOption,
        };

        command.Handler = CommandHandler.Create<string>(async (userId) =>
        {
            UserService userService = di.GetRequiredService<UserService>();
            var events = await userService.GetEvents(userId);
            
            string[] headers = { "Type", "Properties" };
            List<string[]> rows = new();
                    
            foreach (var @event in events)
            {
                rows.Add(new string[]
                {
                    @event.GetType().ToString(),
                    EventProperties(@event)
                });
            }

            UserManagementCli.PrintTable(headers, rows);
        });

        return command;
    }

    private static string EventProperties(IEvent @event)
    {
        Type type = @event.GetType();
        PropertyInfo[] properties = type.GetProperties();

        StringBuilder builder = new StringBuilder();
        foreach (PropertyInfo property in properties)
        {
            builder.Append(property.Name);
            builder.Append(": ");
            builder.Append(property.GetValue(@event, null));
            builder.Append(", ");
        }

        if (builder.Length > 2)
        {
            builder.Length -= 2;
        }

        return builder.ToString();
    }
}