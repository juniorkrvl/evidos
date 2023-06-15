using System.CommandLine;
using System.CommandLine.Invocation;
using Evidos.Assignment.Users.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Evidos.Assignment.ConsoleApp.CommandLineInterface;

internal static class GetUserByEmailCommand
{
    internal static Command Setup(ServiceProvider di)
    {
        var queryOption = new Option<string>(new string[] { "--email", "-e" }, description: "Get user by email")
        {
            Required = true
        };
            
        var command = new Command("get", "Retrieve user by email")
        {
            queryOption,
        };

        command.Handler = CommandHandler.Create<string>(async (email) =>
        {
            UserService userService = di.GetRequiredService<UserService>();
            var user = await userService.GetUserByEmail(email);
            string[] headers = { "Id", "Name", "Email", "Address", "Created At", "Verified At" };
            List<string[]> rows = new ()
            {
                new []
                {
                    user.Id.ToString(), 
                    user.Name, 
                    user.Email.ToString(), 
                    user.Address, 
                    user.CreatedAt.ToString(), 
                    user.VerifiedAt.ToString()
                }
            };

            UserManagementCli.PrintTable(headers, rows);
        });

        return command;
    }
    
}