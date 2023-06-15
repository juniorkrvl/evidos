using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using Evidos.Assignment.ConsoleApp.CommandLineInterface;
using Evidos.Assignment.Users.Services;
using Microsoft.Extensions.DependencyInjection;
using AssignmentBoostrapper = Evidos.Assignment.Bootstrapper;

namespace Evidos.Assignment.ConsoleApp;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Evidos User Management");

        var serviceProvider = BootstrapServices();

        var command = UserManagementCli.SetupCommand(serviceProvider);
        var parser = new CommandLineBuilder(command).UseDefaults().Build();

        await parser.InvokeAsync(args);
    }

    private static ServiceProvider BootstrapServices() 
    {
        var services = new ServiceCollection();
        
        AssignmentBoostrapper.BootstrapServices(services);
        services.AddTransient<UserService>();

        return services.BuildServiceProvider();
    }
}