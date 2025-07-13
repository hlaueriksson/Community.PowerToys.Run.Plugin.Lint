using Community.PowerToys.Run.Plugin.Lint;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NReco.Logging.File;
using Spectre.Console;

var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

using var factory = LoggerFactory.Create(builder => builder.AddFile("ptrun-lint.log", append: true));
var logger = factory.CreateLogger<Program>();

return await AnsiConsole.Status()
    .StartAsync("Linting", async _ =>
    {
        logger.LogInformation("Linting: {Args}", args);

        var worker = new Worker(args, config, logger);

        // Events
        worker.ValidationRule += (sender, e) => Log($"{e.Rule.Code.ToCode()} {e.Rule.Description}");
        worker.ValidationMessage += (sender, e) => Log($" {"-".ToDimmed()} {e.Message}");

        try
        {
            return await worker.RunAsync();
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
            logger.LogError(ex, "RunAsync failed.");
            return ex.HResult;
        }
    });

void Log(string message)
{
    AnsiConsole.MarkupLine(message);
    logger.LogInformation(Markup.Remove(message));
}
