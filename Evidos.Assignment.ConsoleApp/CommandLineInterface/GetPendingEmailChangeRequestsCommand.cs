using System.CommandLine;
using System.CommandLine.Invocation;
using Evidos.Assignment.Users.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Evidos.Assignment.ConsoleApp.CommandLineInterface;

public static class GetPendingEmailChangeRequestsCommand
{
    public static Command Setup(ServiceProvider di)
    {
        var command = new Command("pending-changes", "Retrieve list of pending email change requests");

        command.Handler = CommandHandler.Create(async () =>
        {
            UserService userService = di.GetRequiredService<UserService>();
            var pendingEmailChanges = await userService.GetPendingEmailChanges();
            
            string[] headers = { "Token", "UserId", "Email" };
            List<string[]> rows = new();
                    
            foreach (var emailChange in pendingEmailChanges)
            {
                rows.Add(new string[]
                {
                    emailChange.Token.ToString(),
                    emailChange.UserId.ToString(),
                    emailChange.NewEmail.ToString()
                });
            }

            UserManagementCli.PrintTable(headers, rows);
        });

        return command;
    }
}