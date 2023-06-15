using System.CommandLine;
using System.CommandLine.Invocation;
using Evidos.Assignment.Users.Models;
using Evidos.Assignment.Users.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Evidos.Assignment.ConsoleApp.CommandLineInterface;

public static class RequestEmailChangeCommand
{
    internal static Command Setup(ServiceProvider di)
    {
        var userIdOption = new Option<string>(new string[] { "--user-id", "-id" }, description: "User id")
        {
            Required = true
        };
        
        var newEmailOption = new Option<string>(new string[] { "--email", "-e" }, description: "New email")
        {
            Required = true
        };
            
        var command = new Command("update-email", "Request email change")
        {
            userIdOption,
            newEmailOption,
        };

        command.Handler = CommandHandler.Create<string, string>(async (userId, email) =>
        {
            UserService userService = di.GetRequiredService<UserService>();
            await userService.RequestEmailChange(userId, Email.FromString(email));
        });

        return command;
    }
}