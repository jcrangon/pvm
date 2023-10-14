using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PhpVersionManager
{
    internal class Utils
    {
        static int tableWidth = 73;

        public static void makeTables(string col1, string col2, string[] arr1, string[] arr2)
        {
            PrintLine();
            PrintRow(col1, col2);
            PrintLine();
            int l = arr1.Length > arr2.Length ? arr1.Length : arr2.Length;
            for (int i = 0; i < l; i++)
            {
                dynamic left = "";
                dynamic right = "";
                if (i < arr1.Length)
                {
                    left = arr1[i];
                }
                if (i < arr2.Length)
                {
                    right = arr2[i];
                }
                PrintRow(left, right);

            }
            PrintLine();


        }

        static void PrintLine()
        {
            Console.WriteLine(new string('-', tableWidth));
        }

        static void PrintRow(params string[] columns)
        {
            int width = (tableWidth - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            Console.WriteLine(row);
        }

        static string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }

        public static async Task ListVersions()
        {
            Dictionary<string, string> paths = GetFolderPaths();
            string phpVersionsFolder = paths["phpVersionsFolder"];
            string activeVersionFile = paths["activeVersionFile"];
            string activeVersion = "";
            string activated = "* ";

            if (File.Exists(activeVersionFile))
            {
                activeVersion = await File.ReadAllTextAsync(activeVersionFile);
                
            }

            // si le dossier phpVersions existe
            if (Directory.Exists(phpVersionsFolder))
            {
                // On récup la liste des sous-dossiers du dossier PhpVersions
                string[] sousDossiers = Directory.GetDirectories(phpVersionsFolder);

                // si toutes les versions ont été supprimées
                if (sousDossiers.Length == 1 && Path.GetFileName(sousDossiers[0]) == "activeVersion")
                {
                    Console.WriteLine("");
                    Console.WriteLine("No PHP versions installed!");
                    Console.WriteLine("");
                    return;
                }

                // s'il existe des dossiers d'indtallation PHP
                if (sousDossiers.Length > 0)
                {
                    Console.WriteLine("");
                    foreach (string sousDossier in sousDossiers)
                    {
                        if(Path.GetFileName(sousDossier) != "activeVersion")
                        {
                            if (activeVersion == Path.GetFileName(sousDossier))
                            {
                                Console.WriteLine(activated + Path.GetFileName(sousDossier));
                            }
                            else
                            {
                                Console.WriteLine("  " + Path.GetFileName(sousDossier));
                            }
                        }
                        
                    }
                    Console.WriteLine("");
                    if(activeVersion == "")
                    {
                        Console.WriteLine("Activate PHP with the following command: 'pvm use <version>' to activate that PHP version on your system.");
                        Console.WriteLine("");
                    }
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("No PHP versions installed!");
                    Console.WriteLine("");
                }
            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("No PHP versions installed!");
                Console.WriteLine("");
            }
        }

        public static Dictionary<string, string> GetFolderPaths() 
        {
            Dictionary<string, string> paths = new Dictionary<string, string>();
            paths["activeVersionFileName"] = "activeVersion.actv";
            paths["appDataLocalFolder"] = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            paths["phpVersionsFolder"] = Path.Combine(paths["appDataLocalFolder"], "PhpVersions");
            paths["phpActiveVersion"] = Path.Combine(paths["phpVersionsFolder"], "activeVersion");
            paths["activeVersionFile"] = Path.Combine(paths["phpActiveVersion"], paths["activeVersionFileName"]);

            return paths;
        }

        private static bool GrantAccess(string fullPath)
        {
            var dInfo = new DirectoryInfo(fullPath);
            var dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
            return true;
        }

        // mettre à jour la variable d'environnement
        private static void updatePathEnvVariable(string newActiveVersion, string envPathFinal, string user)
        {
            Dictionary<string, string> paths = GetFolderPaths();
            string file = paths["activeVersionFile"];
            string phpVersionsFolder = paths["phpVersionsFolder"];
            //GrantAccess(phpVersionsFolder);
            

            if (Directory.Exists(phpVersionsFolder))
            {
                // chemin actuel de la variable d'environnement PATH
                string path = user == "user" ? 
                    Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.User) ?? throw new Exception("Env Path empty")
                    :
                    Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.Machine) ?? throw new Exception("Env Path empty");
                //debug:
                //Console.WriteLine(path);
                string[] envPaths = path.Split(';');


                // Créez une nouvelle liste de chemins sans le chemin de PhpVersionsFolder
                var newPaths = new System.Collections.Generic.List<string>();
                bool pathExists = false;

                foreach (var envPath in envPaths)
                {
                    if (envPath.IndexOf(phpVersionsFolder) == -1)
                    {
                        newPaths.Add(envPath);
                    }
                    else
                    {
                        if (envPath.Equals(envPathFinal, StringComparison.OrdinalIgnoreCase))
                        {
                            pathExists = true;
                            break;
                        }

                    }
                }

                if (pathExists)
                {
                    // Console.WriteLine("");
                    // Console.WriteLine($"PHP version {newActiveVersion} id already activated for {(user == "user" ? "User" : "Systeme")}!");
                    // Console.WriteLine("");
                    return;
                }

                // On join les chemins restants pour former la nouvelle valeur de PATH
                string newEnvPath = string.Join(";", newPaths);


                // Mettez à jour la variable d'environnement PATH
                if (user == "user")
                {
                    try
                    {
                        // on place le chemin en tete
                        newEnvPath = envPathFinal + ";" + newEnvPath;
                        Environment.SetEnvironmentVariable("PATH", newEnvPath, EnvironmentVariableTarget.User);
                        Console.WriteLine("");
                        Console.WriteLine("PHP successfully installed for current user!");
                        //if (!IsAdministrator())
                        //{
                        //    Console.WriteLine("");
                        //    Console.WriteLine("PHP could not be installed at system level though.");
                        //    Console.WriteLine("To install at system level open the terminal with administrator privileges...");
                        //}
                    }
                    catch
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Error! PHP could not be installed!");
                        Console.WriteLine("");
                    }
                }
                // else if (user == "machine" && IsAdministrator())
                //{
                //    try
                //    {
                //        // on place le chemin en tete
                //        newEnvPath = envPathFinal + ";" + newEnvPath;
                //        Environment.SetEnvironmentVariable("PATH", newEnvPath, EnvironmentVariableTarget.Machine);
                //        Console.WriteLine("");
                //        Console.WriteLine("PHP successfully installed at system level!");
                //        Console.WriteLine("");
                //    } catch (Exception e) 
                //    {
                //        Console.WriteLine("");
                //        Console.WriteLine("Error! Your terminal must have administrator privileges to install PHP at system level...");
                //        Console.WriteLine("");
                //    }
                    
                //}
                
            }
            else
            {
                Console.WriteLine("Le dossier PhpVersions n'existe pas.");
            }
        }

        public static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        // active la version choisie
        public static async Task setActiveVersion (string newActiveVersion) 
        {
            //if (!IsAdministrator())
            //{
            //    Console.WriteLine("");
            //    Console.WriteLine("Activation IMPOSSIBLE du to lack of ADMIN PRIVILEGES...");
            //    Console.WriteLine("Please open your terminal WITH administrator privileges.");
            //    Console.WriteLine("");
            //    return;
            //}

            newActiveVersion = newActiveVersion.Trim();

            Dictionary<string, string> paths = GetFolderPaths();
            string file = paths["activeVersionFile"];
            string phpVersionsFolder = paths["phpVersionsFolder"];
            Directory.CreateDirectory(paths["phpActiveVersion"]);
            string envPathFinal = Path.Combine(phpVersionsFolder, newActiveVersion);
            //GrantAccess(paths["phpActiveVersion"]);

            if (!Directory.Exists(envPathFinal))
            {
                Console.WriteLine("");
                Console.WriteLine("ERROR: version unknown ...");
                Console.WriteLine("This version is not installed");
                Console.WriteLine("");
                return;
            }

            updatePathEnvVariable(newActiveVersion, envPathFinal, "user");
            //updatePathEnvVariable(newActiveVersion, envPathFinal, "machine");

            try
            {
                await File.WriteAllTextAsync(file, newActiveVersion);
                Console.WriteLine("Now using PHP version " + newActiveVersion);
                Console.WriteLine("");
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine($"An error occurred while activating php Version : {ex.Message}");
                Console.WriteLine("");
            }
        }

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

        public static async Task RemoveVersion(string version)
        {
            Dictionary<string, string> paths = GetFolderPaths();
            string phpVersionsFolder = paths["phpVersionsFolder"];
            string file = paths["activeVersionFile"];
            string activeVersion = "";

            if (File.Exists(file))
            {
                activeVersion = await File.ReadAllTextAsync(file);

            }

            if (activeVersion == version)
            {
                try
                {
                    await File.WriteAllTextAsync(file, "");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("");
                    Console.WriteLine($"An error occurred while emptying active version file : {ex.Message}");
                    Console.WriteLine("");
                }
            }

            if (Directory.Exists(Path.Combine(phpVersionsFolder, version)))
            {
                await DeleteDirectoryAsync(Path.Combine(phpVersionsFolder, version));

            } else
            {

                Console.WriteLine("Le dossier PhpVersions n'existe pas.");
                Console.WriteLine("... Done!");
                Console.WriteLine("");
            }
        }

        public static void DisplayHelp()
        {
            Console.WriteLine("   list available     - List available PHP versions");
            Console.WriteLine("   ls                 - List installed versions of PHP");
            Console.WriteLine("   install [version]  - Installs the chosen PHP [version]");
            Console.WriteLine("   use [version]      - Activates the chosen PHP [version]");
            Console.WriteLine("   remove [version]   - Removes the chosen PHP [version]");
            Console.WriteLine("   display ini        - Displays the php.ini of the installed version");
            Console.WriteLine("");
            Console.WriteLine("");
        }

        public static async Task DisplayIni()
        {
            Dictionary<string, string> paths = GetFolderPaths();
            string versionName;
            string pathToVersion;
            string file = paths["activeVersionFile"];

            if (File.Exists(file))
            {
                try
                {
                    versionName = await File.ReadAllTextAsync(file);
                    if (versionName != "")
                    {
                        pathToVersion = Path.Combine(paths["phpVersionsFolder"], versionName);
                    }
                    else
                    {
                        Console.WriteLine("No PHP is activated on your system. Please activate a version of PHP");
                        Console.WriteLine("PHP version(s installed on your system:");
                        await ListVersions();
                        Console.WriteLine("Enter command: 'use [version]' to activate a version of PHP");
                        return;
                    }

                    Console.WriteLine(pathToVersion);

                    string answer = "";
                    Console.WriteLine("Choose what editor you want to use:");
                    Console.WriteLine("Note pad    [1]");
                    Console.WriteLine("Notepad++   [2]");
                    Console.WriteLine("VSCode      [3]");
                    Console.Write("Votre choix....");
                    var keyInfo = Console.ReadKey(intercept: true);
                    answer += keyInfo.KeyChar;

                    switch (answer)
                    {
                        case "1":
                            Console.WriteLine($"[{answer}]");
                            Console.WriteLine("Opening with notepad ...");
                            try
                            {
                                Process.Start("notepad.exe", Path.Combine(pathToVersion, "php.ini"));
                            } catch 
                            {
                                Console.WriteLine("Unable to find notepad.exe on your system. Please check if it is installed in PATH for current user");
                            }
                            
                            break;

                        case "2":
                            Console.WriteLine($"[{answer}]");
                            Console.WriteLine("Opening with notepad++ ...");
                            try
                            {
                                var pi = new ProcessStartInfo
                                {
                                    UseShellExecute = true,
                                    FileName = "notepad++",
                                    Arguments = Path.Combine(pathToVersion, "php.ini"),
                                    WindowStyle = ProcessWindowStyle.Hidden
                                };
                                Process.Start(pi);
                            }
                            catch 
                            {
                                try
                                {
                                    Console.WriteLine("Unable to find notepad++.exe on your system. Opening php.ini with NotePad");
                                    Process.Start("notepad.exe", Path.Combine(pathToVersion, "php.ini"));
                                }
                                catch
                                {
                                    Console.WriteLine("Unable to find notepad.exe on your system. Please check if it is installed in PATH for current user");
                                }

                            }
                            break;

                        case "3":
                            Console.WriteLine($"[{answer}]");
                            Console.WriteLine("Opening with VSCode ...");
                            try
                            {
                                var pi = new ProcessStartInfo
                                {
                                    UseShellExecute = true,
                                    FileName = "code",
                                    Arguments = Path.Combine(pathToVersion, "php.ini"),
                                    WindowStyle = ProcessWindowStyle.Hidden
                                };
                                Process.Start(pi);
                            }
                            catch
                            {
                                try
                                {
                                    Console.WriteLine("Unable to find code.exe on your system. Opening php.ini with NotePad...");
                                    Process.Start("notepad.exe", Path.Combine(pathToVersion, "php.ini"));
                                } catch 
                                {
                                    Console.WriteLine("Unable to find notepad.exe on your system. Please check if it is installed in PATH for current user");
                                } 
                                
                            }
                            break;

                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: " + e.Message);
                } 

            }
            else
            {
                Console.WriteLine("");
                Console.WriteLine("WARNING: Please Activate a version of PHP first!");
                Console.WriteLine("Enter command: 'use [version]' to activate a version of PHP");
            } 

        }
    }
}
