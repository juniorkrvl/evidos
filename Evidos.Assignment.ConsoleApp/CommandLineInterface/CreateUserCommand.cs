using System.CommandLine;
using System.CommandLine.Invocation;
using Evidos.Assignment.Users.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Evidos.Assignment.ConsoleApp.CommandLineInterface;

internal static class CreateUserCommand
{
    internal static Command Setup(ServiceProvider di)
    {
        var nameOption = new Option<string>(new string[] { "--name", "-n" }, description: "Name of the user")
        {
            Required = true
        };

        var emailOption = new Option<string>(new string[] { "--email", "-e" }, description: "Email of the user")
        {
            Required = true
        };

        var addressOption = new Option<string>(new string[] { "--address", "-a" }, description: "Address of the user");

        var command = new Command("create", "Create a new user")
        {
            nameOption,
            emailOption,
            addressOption
        };

        command.Handler = CommandHandler.Create<string, string, string>(async (name, email, address) =>
        {
            UserService userService = di.GetRequiredService<UserService>();
            await userService.CreateUser(name, email, address);
        });

        return command;
    }
    
}