// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using ZipTransfer.Services;

Console.WriteLine("ZipTransfer");
Console.WriteLine(string.Empty);

//  use a json config file
// zip folders
var loggerService = new LoggerService();
var argService = new ArgService(loggerService);
var stopwatch = new Stopwatch();

try
{
    // minimal assistance
    if (args.Length > 0 && (args[0] == "-?" || args[0] == "-help" || args[0] == "?" || args[0] == "help"))
    {
        Console.WriteLine(string.Empty);
        Console.WriteLine("Pass a path as the first parameter to zip each of its subdirectories and transfer those zip files to a path passed as the second parameter.");
        Console.WriteLine("If paths are not passed as arguments then the Configuration.json provides the sources to zip and the zip files' destination.");
        Console.WriteLine(string.Empty);
        Console.WriteLine(argService.GetHelpText());
        Console.WriteLine(string.Empty);
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        return;
    }

    var versionService = new VersionService(loggerService);
    var zipService = new ZipService(loggerService, versionService);
    var configService = new ConfigService(loggerService);
    var configuration = await configService.GetConfiguration();

    stopwatch.Start();

    if (args.Length >= 2)
    {
        // if a folder was provided then zip all of its subdirectories into separate zip files
        // the provided directory is also where the zip files will be created before being moved to the destination.
        // TODO: support for versioning the zip files
        argService.ParseArgs(args);

        int versions = 0;
        int.TryParse(argService.GetArgValueByTitle("versions"), out versions);

        zipService.ZipSubdirectoriesAndMoveToDestination(argService.GetArgValueByTitle("source"), argService.GetArgValueByTitle("destination"), argService.GetArgValueByTitle("temp"), versions);

        //-s "C:\temp\TestTransfers\source" -d "C:\temp\TestTransfers\destination" -t "C:\temp\TestTransfers\temp"
    }
    else if (args.Length == 0)
    {
        // use the configuration file to determine which directories to zip
        zipService.ZipConfiguredPathsAndMoveToDestination(configuration.Transfers, configuration.TempLocation);
    }
    else
    {
        loggerService.WriteError("I'm not sure what you are trying to accomplish.");
        return;
    }
    
    stopwatch.Stop();
    Console.WriteLine("");
    loggerService.WriteLine($"Process complete. Elapsed time: {stopwatch.Elapsed.TotalSeconds} seconds");
    Console.WriteLine("Press any key to exit");
    Console.ReadKey();
}
finally
{
    loggerService.EndLog();
}

