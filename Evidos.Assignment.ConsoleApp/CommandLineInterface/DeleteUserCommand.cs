using System.CommandLine;
using System.CommandLine.Invocation;
using Evidos.Assignment.Users.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Evidos.Assignment.ConsoleApp.CommandLineInterface;

internal static class DeleteUserCommand
{
    internal static Command Setup(ServiceProvider di)
    {
        var queryOption = new Option<string>(new string[] { "--user-id", "-id" }, description: "Delete user by id")
        {
            Required = true
        };
            
        var command = new Command("delete", "Delete user private data")
        {
            queryOption,
        };

        command.Handler = CommandHandler.Create<string>(async (userId) =>
        {
            UserService userService = di.GetRequiredService<UserService>();
            await userService.DeleteUser(userId);
        });

        return command;
    }
}