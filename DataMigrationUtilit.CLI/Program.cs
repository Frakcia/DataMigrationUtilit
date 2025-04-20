using DataMigrationUtilit.Application.Contracts;
using DataMigrationUtilit.Application.Handlers.GetData;
using DataMigrationUtilit.Application.Serivces;
using DataMigrationUtilit.CLI.DependecyInjection;
using DataMigrationUtilit.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;

class Program
{
    static async Task Main(string[] args)
    {
        var host = CreateHost(args);

        var dbService = host.Services.GetRequiredService<DbService>();

        dbService.ApplyMigrations();

        Console.WriteLine("Для импорта введите: Import fileName entityType");
        Console.WriteLine("Для вывода введите: Out departmentId");

        while (true)
        {
            var input = Console.ReadLine();
            var isEmptyInput = string.IsNullOrWhiteSpace(input);

            if (!isEmptyInput && input.StartsWith("Out"))
            {
                await OutPutAction(host, input);

            }
            else if (!isEmptyInput && input.StartsWith("Import"))
            {
                await ImportAction(host, input);
            }
            else
            {
                Console.WriteLine("Не удалось распознать команду");
            }

            Console.WriteLine("Ожидание следующей команды:");
        }
    }

    static async Task ImportAction(IHost host, string input)
    {
        var imtportAction = host.Services.GetRequiredService<IImportDataService>();

        var inputArgs = input.Split(' ');

        if (inputArgs.Length > 0)
        {
            await imtportAction.Import(inputArgs[1], inputArgs[2]);
            Console.WriteLine("Импорт данных завершен");
        }
        else
        {
            Console.WriteLine("Передано не достаточно аргументов");
        }
    }

    static async Task OutPutAction(IHost host, string input)
    {
        var getDataHandler = host.Services.GetRequiredService<GetDataHandler>();

        var inputArgs = input.Split(' ');
        long? departmentId = null;

        if (inputArgs.Length >= 2)
        {
            if (long.TryParse(inputArgs[1], out var inputDepartmentId))
            {
                departmentId = inputDepartmentId;
            }
            else
            {
                Console.WriteLine($"Параметр не является валидным DepartmentId: {inputArgs[1]}");
                return;
            }
        }

        var data = await getDataHandler.GetData(departmentId);

        ShowOutPut(data, 1);

        Console.WriteLine();
        Console.WriteLine("Вывод данных завершен");
    }

    static void ShowOutPut(IEnumerable<DepartmentOutDto> departments, int level)
    {
        foreach (var department in departments)
        {
            var departmentPrefix = string.Concat(Enumerable.Repeat("=", level));
            Console.WriteLine($"{departmentPrefix} {department.Name} ID = {department.Id}");

            var spaces = string.Concat(Enumerable.Repeat(" ", level));
            var employeePrefix = "*";

            var bossEmployee = department.Employees.FirstOrDefault(e => e.Id == department.ManagerId);
            if (bossEmployee != null)
            {
                Console.WriteLine($"{spaces}{employeePrefix} {bossEmployee.FullName} ID={bossEmployee.Id} ({bossEmployee.JobTitleName} ID={bossEmployee.JobTitleId})");
            }

            foreach (var employee in department.Employees.Where(e => e.Id != department.ManagerId))
            {
                employeePrefix = "-";
                Console.WriteLine($"{spaces}{employeePrefix} {employee.FullName} ID={employee.Id} ({employee.JobTitleName} ID={employee.JobTitleId})");
            }

            if (department.Departments.Any())
            {
                level++;
                ShowOutPut(department.Departments, level);
                level--;
            }
        }
    }

    static IHost CreateHost(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.SetBasePath(FileService.BaseDir);
                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((context, services) =>
                {
                    services
                     .AddConsoleLogging()
                     .AddDbContext(context)
                     .RegisterServices();
                })
                .Build();

        return host;
    }
}