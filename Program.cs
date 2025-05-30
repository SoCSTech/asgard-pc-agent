using asgard_pc_agent;
using CliWrap;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

const string ServiceName = "Asgard PC Agent";

if (args is { Length: 1 })
{
    try
    {
        string executablePath =
            Path.Combine(AppContext.BaseDirectory, "asgard-pc-agent.exe");

        if (args[0] is "/Install")
        {
            await Cli.Wrap("sc")
                .WithArguments(new[] { "create", ServiceName, $"binPath={executablePath}", "start=auto" })
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync();

            await Cli.Wrap("sc")
                .WithArguments(new[] { "start", ServiceName })
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync();
        }
        else if (args[0] is "/Uninstall")
        {
            await Cli.Wrap("sc")
                .WithArguments(new[] { "stop", ServiceName })
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync();

            await Cli.Wrap("sc")
                .WithArguments(new[] { "delete", ServiceName })
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex);
    }

    return;
}

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = ServiceName;
});

LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(builder.Services);

var host = builder.Build();
host.Run();
