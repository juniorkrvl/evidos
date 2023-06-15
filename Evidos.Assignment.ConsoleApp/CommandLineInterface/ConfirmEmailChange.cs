using System.CommandLine;
using System.CommandLine.Invocation;
using Evidos.Assignment.Users.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Evidos.Assignment.ConsoleApp.CommandLineInterface;

public static class ConfirmEmailChange
{
    internal static Command Setup(ServiceProvider di)
    {
        var tokenOption = new Option<string>(new string[] { "--token", "-t" }, description: "Email change token")
        {
            Required = true
        };
        
        var command = new Command("confirm-change", "Confirm email change")
        {
            tokenOption,
        };

        command.Handler = CommandHandler.Create<string>(async (token) =>
        {
            UserService userService = di.GetRequiredService<UserService>();
            await userService.ConfirmEmailChange(token);
        });

        return command;
    }
}