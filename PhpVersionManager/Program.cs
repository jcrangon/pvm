using HtmlAgilityPack;
using System;

namespace PhpVersionManager
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            if (!Convert.ToBoolean(args.Length))
            {
                bool installed = false;

                string? installDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string? path = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);
                Dictionary<string, string> paths = new Dictionary<string, string>();
                paths["appDataLocalFolder"] = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                paths["pvmSetup"] = Path.Combine(paths["appDataLocalFolder"], "pvm-setup");
                Directory.CreateDirectory(paths["pvmSetup"]);

                if (installDir != null && File.Exists(Path.Combine(paths["pvmSetup"], "pvm.log")))
                {
                    installed = true;
                    Console.WriteLine("Type 'pvm help' to get a list of all PVM commands!");
                }

                if (!installed && path != null && installDir != null)
                {

                    if (!path.Contains(installDir))
                    {
                        try
                        {
                            await File.WriteAllTextAsync(Path.Combine(paths["pvmSetup"], "pvm.log"), installDir);

                        } catch(Exception e)
                        {
                            Console.WriteLine(e.Message);
                            
                        }
                        

                        string envPath = $"{installDir};{path}";

                        Environment.SetEnvironmentVariable("PATH", envPath, EnvironmentVariableTarget.User);
                        Console.WriteLine("configuring pmv....... Done!");
                        Console.WriteLine("Press any key to exit.");
                        Console.ReadKey();
                    } else
                    {
                        Console.WriteLine("Pvm is correctly configured.");
                        Console.WriteLine("Press any key to exit.");
                    }

                    
                }

            } else
            {
                switch (args[0])
                { 
                    case "list":
                        if (args[1].Length != 0 && args[1] == "available")
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Listing all available versions");
                            Web.Harvester();

                        }
                        break;
                    case "ls":
                        Console.WriteLine("");
                        Console.WriteLine("Listing all installed versions...");
                        await Utils.ListVersions();   
                        break;
                    case "install":
                        if (args[1].Length != 0)
                        {
                            string version = args[1];
                            Console.WriteLine("");
                            Console.WriteLine($"Installing version {version}");
                             await Downloader.PhpDownload(version);
                        }
                        break;
                    case "use":
                        if (args[1].Length != 0)
                        {
                            Console.WriteLine("");
                            string version = args[1];
                            Console.WriteLine($"Activating version {version}...");
                            await Utils.setActiveVersion(version);
                        }
                        break;
                    case "remove":
                        if (args[1].Length != 0)
                        {
                            Console.WriteLine("");
                            string version = args[1];
                            Console.WriteLine($"Removing version {version}...");
                            await Utils.RemoveVersion(version);
                        }
                        break;

                    case "display":
                        if (args[1].Length != 0 && args[1] == "ini")
                        {
                            Console.WriteLine("");
                            Console.WriteLine($"Displaying php.ini...");
                            await Utils.DisplayIni();
                        }
                        break;

                    case "-help":
                        Console.WriteLine("");
                        Console.WriteLine("Printing PVM help...");
                        Utils.DisplayHelp();
                        break;

                    case "-h":
                        Console.WriteLine("");
                        Console.WriteLine("Printing PVM help...");
                        Utils.DisplayHelp();
                        break;
                }
            }
            
        }
    }
}