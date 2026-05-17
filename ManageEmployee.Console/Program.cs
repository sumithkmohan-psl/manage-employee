using ManageEmployee.Business.Services;
using ManageEmployee.Business.Validators;
using ManageEmployee.Data.Connection;
using ManageEmployee.Data.Repositories;
using ManageEmployee.Models.DTOs;
using ManageEmployee.Models.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using System.Text.RegularExpressions;

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile(
        "appsettings.json",
        optional: true)
    .AddEnvironmentVariables()
    .Build();

var connectionString =
    configuration.GetConnectionString("DefaultConnection");

if (connectionString == null)
{
    Console.WriteLine("Connection string not found.");
    return;
}

var services = new ServiceCollection();

services.AddSingleton<IConnectionFactory>(x => new SqlConnectionFactory(connectionString));

services.AddScoped<IEmployeeRepository, EmployeeRepository>();

services.AddScoped<IEmployeeService, EmployeeService>();

var serviceProvider = services.BuildServiceProvider();

var service = serviceProvider.GetRequiredService<IEmployeeService>();

if (args.Length == 0)
{
    Console.WriteLine("Invalid command");
    return;
}

var command = args[0].ToLower();

try
{
    switch (command)
    {
        case "-list":
            if (args.Length == 1)
            {
                await ListEmployees();
            }
            else
            {
                await ListEmployees(args[1]);
            }
            break;
        case "-titles":
            await ListJobTitles();
            break;
        case "-add":
            await AddEmployee();
            break;
        default:
            Console.WriteLine("Unknown command");
            break;
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);

    LogError(ex);
}

async Task AddEmployee()
{
    var employee = new EmployeeDto();

    employee.Name = await ReadString("Name");

    employee.SSN = await ReadString("SSN");

    employee.DOB = ReadDate("DOB");

    employee.Address = await ReadString("Address");

    employee.City = await ReadString("City");

    employee.State = await ReadString("State");

    employee.Zip = await ReadString("Zip");

    employee.Phone = await ReadString("Phone");

    employee.JoinDate = ReadDate("Join Date", employee.DOB);

    employee.Title = await ReadString("Title");

    employee.Salary = ReadDecimal("Salary");

    await service.AddEmployee(employee);

    Console.WriteLine("Employee added successfully.");
}

decimal ReadDecimal(string fieldName)
{
    while (true)
    {
        Console.WriteLine($"Enter {fieldName}:");
        var input = Console.ReadLine() ?? string.Empty;

        if (!EmployeeValidator.IsValidSalary(input, out var value))
        {
            Console.WriteLine($"Invalid {fieldName} format. Please enter a valid decimal number with up to 12 digits.");
            continue;
        }

        return value;
    }
}

DateTime ReadDate(string fieldName, DateTime? dob = null)
{
    while (true)
    {
        Console.WriteLine($"Enter {fieldName} (yyyy-MM-dd):");

        var input = Console.ReadLine() ?? string.Empty;

        if (!EmployeeValidator.IsValidDate(input, out var date))
        {
            Console.WriteLine($"Invalid {fieldName} format. Please use yyyy-MM-dd.");
            continue;
        }

        if (fieldName == "DOB" && !EmployeeValidator.IsValidDOB(date))
        {
            Console.WriteLine($"Invalid {fieldName}. Age must be between 22 and 64 years.");
            continue;
        }

        if (fieldName == "Join Date" && !EmployeeValidator.IsValidJoinDate(date, dob ?? DateTime.MinValue))
        {
            Console.WriteLine("Invalid Join Date. It must be between 22 and 64 years after the DOB and cannot be in the future.");
            continue;
        }

        return date;
    }
}

async Task<string> ReadString(string fieldName)
{
    while (true)
    {
        Console.WriteLine($"Enter {fieldName}:");
        var input = Console.ReadLine() ?? string.Empty;
        if (EmployeeValidator.IsEmpty(input))
        {
            Console.WriteLine($"{fieldName} is required.");
            continue;
        }

        if (fieldName == "Name" && !EmployeeValidator.IsValidName(input))
        {
            Console.WriteLine($"Invalid {fieldName} format. Please enter 3 to 100 alphabetic characters.");
            continue;
        }

        if (fieldName == "SSN")
        {
            if (!EmployeeValidator.IsValidSSN(input))
            {
                Console.WriteLine("Invalid SSN format. Please enter a valid SSN (e.g., 123-45-6789).");
                continue;
            }

            var isUnique = await service.IsUniqueSSN(input);

            if (!isUnique)
            {
                Console.WriteLine("SSN already exists. Please enter a unique SSN.");
                continue;
            }
        }

        if (fieldName == "Address" && !EmployeeValidator.IsValidAddress(input))
        {
            Console.WriteLine($"Invalid {fieldName} format. Please enter 5 to 200 alphanumeric characters.");
            continue;
        }

        if ((fieldName == "City" || fieldName == "State") && input.Length > 100)
        {
            Console.WriteLine($"{fieldName} cannot exceed 100 characters.");
            continue;
        }

        if (fieldName == "City" && !EmployeeValidator.IsValidCity(input))
        {
            Console.WriteLine($"Invalid {fieldName} format. Please enter 2 to 100 alphabetic characters.");
            continue;
        }

        if (fieldName == "State" && !EmployeeValidator.IsValidState(input))
        {
            Console.WriteLine($"Invalid {fieldName} format. Please enter 2 to 100 alphabetic characters.");
            continue;
        }
        if (fieldName == "Zip" && !EmployeeValidator.IsValidZip(input))
        {
            Console.WriteLine("Invalid Zip code format. Please enter a valid Zip code (e.g., 12345 or 12345-6789).");
            continue;
        }

        if (fieldName == "Phone" && !EmployeeValidator.IsValidPhone(input))
        {
            Console.WriteLine("Invalid Phone number format. Please enter a valid phone number (e.g., (123) 456-7890).");
            continue;
        }

        if (fieldName == "Title" && !EmployeeValidator.IsValidTitle(input))
        {
            Console.WriteLine("Invalid Title format. Please enter a valid title (e.g., Manager, Developer).");
            continue;
        }

        return input;
    }
}

async Task ListJobTitles()
{
    var titles = await service.GetJobTitles();

    var table = new Table();

    table.Border(TableBorder.Rounded);

    table.AddColumn("Title");
    table.AddColumn("Min Salary");
    table.AddColumn("Max Salary");

    foreach (var title in titles)
    {
        table.AddRow(title.Name, title.Min.ToString("C"), title.Max.ToString("C"));
    }

    AnsiConsole.Write(table);

    AnsiConsole.MarkupLine(
        $"\n[green]Total Titles:[/] {titles.Count()}");
}

async Task ListEmployees(string? keyword = null)
{
    var employees = await service.GetEmployees(keyword);

    var table = new Table();

    table.Border(TableBorder.Rounded);

    table.AddColumn("Name");
    table.AddColumn("SSN");
    table.AddColumn("DOB");
    table.AddColumn("Address");
    table.AddColumn("City");
    table.AddColumn("State");
    table.AddColumn("Zip");
    table.AddColumn("Phone");
    table.AddColumn("Join Date");
    table.AddColumn("Exit Date");
    table.AddColumn("Salary");
    table.AddColumn("Title");

    foreach (var employee in employees)
    {
        table.AddRow(employee.Name, employee.SSN, employee.DOB.ToShortDateString(), employee.Address, employee.City, employee.State, employee.Zip, employee.Phone, employee.JoinDate.ToShortDateString(), employee.ExitDate?.ToShortDateString() ?? "N/A", employee.Salary.ToString("C"), employee.Title);
    }

    AnsiConsole.Write(table);

    AnsiConsole.MarkupLine(
        $"\n[green]Total Employees:[/] {employees.Count()}");
}

static void LogError(Exception ex)
{
    string logDirectory = Path.Combine(AppContext.BaseDirectory, "Logs");

    Directory.CreateDirectory(logDirectory);

    string filePath = Path.Combine(logDirectory, $"error-{DateTime.Today:yyyy-MM-dd}.txt");

    string message = $@"
[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]
{ex}
----------------------------------------";

    File.AppendAllText(filePath, message);
}