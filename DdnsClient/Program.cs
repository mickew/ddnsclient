using System.Diagnostics;
using System.Reflection;
using DdnsClient.Services;
using Serilog;

namespace DdnsClient;

public class Program
{
    private const string VersionArgs = "--version";
    private const string SerilogOutputTemplate = "[{Timestamp:HH:mm:ss} {SourceContext} [{Level}] {Message}{NewLine}{Exception}";

    private static IConfiguration GetConfiguration(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .AddUserSecrets("dotnet-DdnsClient-4c44f6f6-231c-447d-a0c0-3b20018d3f55")
            .AddCommandLine(args)
            .Build();
        return configuration;
    }

    public static int Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(GetConfiguration(args))
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: SerilogOutputTemplate)
            .CreateBootstrapLogger();
        try
        {
            var printVersion = args.Any(x => x == VersionArgs);
            if (printVersion)
            {
                Console.WriteLine(GetVersion());
                return 0;
            }
            Log.Information("Starting host");

            var host = BuildHost(args);
            host.Run();

            Log.Information("Stopping host");
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
    private static IHost BuildHost(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .UseSystemd()
            .ConfigureHostConfiguration(conf =>
            {
                conf.SetBasePath(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule!.FileName)!);
            })
            .ConfigureServices(services =>
            {
                services.AddHttpClient<IDdnsService>();
                services.AddSingleton<IDdnsService, DdnsService>();
                services.AddHostedService<DdnsUpdateService>();
            })
            .UseSerilog()
            .Build();
    }

    private static string GetVersion()
    {
        Assembly currentAssembly = typeof(Program).Assembly;
        if (currentAssembly == null)
        {
            currentAssembly = Assembly.GetCallingAssembly();
        }
        var version = $"{currentAssembly.GetName().Version!.Major}.{currentAssembly.GetName().Version!.Minor}.{currentAssembly.GetName().Version!.Build}";
        return version ?? "?.?.?";
    }
}