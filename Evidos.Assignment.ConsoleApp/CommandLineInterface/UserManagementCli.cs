using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using Evidos.Assignment.Users;
using Microsoft.Extensions.DependencyInjection;

namespace Evidos.Assignment.ConsoleApp.CommandLineInterface;

internal static class UserManagementCli
{
    
    public static RootCommand SetupCommand(ServiceProvider di)
    {
        var root = new RootCommand("Evidos User Management CLI. Type 'exit' to quit.");
        root.Handler = CommandHandler.Create(() =>
        {
            while (true)
            {
                Console.Write("👤 > ");
                var input = ReadLine.Read();
                if (input == "exit") break;
                if (input == "") continue;
                
                ReadLine.AddHistory(input);

                var cmd = new CommandLineBuilder(root)
                    .UseDefaults()
                    .UseExceptionHandler((exception, context) =>
                    {
                        if (exception is UserException)
                        {
                            Console.WriteLine(exception.Message);
                        }
                        else
                        {
                            Console.WriteLine($"An error occurred: {exception.Message}");
                            throw exception;
                        }
                    })
                    .Build();

                cmd.Invoke(input);
                
                
            }
        });

        root.AddCommand(CreateUserCommand.Setup(di));
        root.AddCommand(DeleteUserCommand.Setup(di));
        root.AddCommand(GetUserByEmailCommand.Setup(di));
        root.AddCommand(SearchUserByEmailCommand.Setup(di));
        root.AddCommand(GetEventsCommand.Setup(di));
        root.AddCommand(GetPendingEmailChangeRequestsCommand.Setup(di));
        root.AddCommand(RequestEmailChangeCommand.Setup(di));
        root.AddCommand(ConfirmEmailChange.Setup(di));
        
        return root;
    }
    
    public static void PrintTable(string[] columnHeaders, List<string[]> rows)
    {
        int[] maxColumnsWidth = GetMaxColumnWidths(columnHeaders, rows);

        string rowFormat = string.Join(
            " | ", 
            columnHeaders.Select((header, index) => $"{{{index},-{maxColumnsWidth[index]}}}")
        );

        Console.WriteLine(string.Format(rowFormat, columnHeaders));
        Console.WriteLine(new string('-', rowFormat.Length));

        foreach (var row in rows)
        {
            Console.WriteLine(string.Format(rowFormat, row));
        }
    }

    private static int[] GetMaxColumnWidths(string[] columnHeaders, List<string[]> rows)
    {
        var maxColumnWidths = columnHeaders.Select(header => header.Length).ToArray();

        foreach (var row in rows)
        {
            for (int i = 0; i < row.Length; i++)
            {
                maxColumnWidths[i] = Math.Max(maxColumnWidths[i], row[i] != null ? row[i].Length: 0);
            }
        }

        return maxColumnWidths;
    }
}