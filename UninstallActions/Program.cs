using System.IO;

namespace UninstallActions
{
    internal class Program
    {
        static async Task DeleteDirectoryAsync(string directoryPath)
        {
            foreach (string file in Directory.GetFiles(directoryPath))
            {
                File.Delete(file);
            }

            foreach (string subDirectory in Directory.GetDirectories(directoryPath))
            {
                await DeleteDirectoryAsync(subDirectory);
            }

            Directory.Delete(directoryPath);
        }


        static async Task Main(string[] args)
        {
            Dictionary<string, string> paths = new Dictionary<string, string>();
            paths["appDataLocalFolder"] = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            paths["pvmSetup"] = Path.Combine(paths["appDataLocalFolder"], "pvm-setup");
            string? allPaths = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);

            if (File.Exists(Path.Combine(paths["pvmSetup"], "pvm.log")))
            {
                string activePath = await File.ReadAllTextAsync(Path.Combine(paths["pvmSetup"], "pvm.log"));
                if (allPaths.Contains(activePath))
                {
                    string[] allPathTable = allPaths.Split(';');
                    var newPaths = new System.Collections.Generic.List<string>();

                    foreach (var p in allPathTable)
                    {
                        
                        if (p.IndexOf(activePath) == -1)
                        {
                            newPaths.Add(p);
                        }

                    }
                    string newEnvPath = string.Join(";", newPaths);
                    Environment.SetEnvironmentVariable("PATH", newEnvPath, EnvironmentVariableTarget.User);
                }

            }
            await DeleteDirectoryAsync(paths["pvmSetup"]);
        }
    }
}