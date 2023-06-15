using System.CommandLine;
using System.CommandLine.Invocation;
using Evidos.Assignment.Users.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Evidos.Assignment.ConsoleApp.CommandLineInterface;

internal class SearchUserByEmailCommand
{
    internal static Command Setup(ServiceProvider di)
    {
        var queryOption = new Option<string>(new string[] { "--query", "-q" }, description: "Email query")
        {
            Required = true
        };
            
        var command = new Command("search", "Search users by email")
        {
            queryOption,
        };

        command.Handler = CommandHandler.Create<string>(async (query) =>
        {
            Console.WriteLine($"Searching for : {query} \n");
            UserService userService = di.GetRequiredService<UserService>();
                
            var users = userService.SearchUserByEmail(query).Result.ToList();
                
            Console.WriteLine($"Users found: {users.Count()} \n");
                
            string[] headers = { "Id", "Name", "Email", "Address", "Created At", "Verified At" };
            List<string[]> rows = new();
                    
            foreach (var user in users)
            {
                rows.Add(new []
                {
                    user.Id.ToString(), 
                    user.Name, 
                    user.Email.ToString(), 
                    user.Address, 
                    user.CreatedAt.ToString(),
                    user.VerifiedAt.ToString()
                });
            }

            UserManagementCli.PrintTable(headers, rows);
            Console.WriteLine();
        });

        return command;
    }
    
}