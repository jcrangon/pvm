using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace PhpVersionManager
{
    internal class Downloader
    {
        private static bool GrantAccess(string fullPath)
        {
            var dInfo = new DirectoryInfo(fullPath);
            var dSecurity = dInfo.GetAccessControl();
            dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
            dInfo.SetAccessControl(dSecurity);
            return true;
        }
        public static async Task PhpDownload(string version)
        {
            string url = "https://windows.php.net/downloads/releases/archives/" + version + ".zip";
            //Console.WriteLine(url);
            string appDataLocalFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string phpVersionsFolder = Path.Combine(appDataLocalFolder, "PhpVersions/" + version);
            string destinationPath = Path.Combine(phpVersionsFolder, version + ".zip");
            string phpIni;

            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    // on cree le dossier de destination
                    Directory.CreateDirectory(phpVersionsFolder);
                    GrantAccess(phpVersionsFolder);

                    Console.WriteLine("");
                    Console.WriteLine("Downloading...");

                    // Si le fichier de destination existe on prend sa taille
                    long existingFileSize = 0;
                    if (File.Exists(destinationPath))
                    {
                        existingFileSize = new FileInfo(destinationPath).Length;
                    }

                    // récup de la plage de téléchargement pour reprendre depuis la fin du fichier existant
                    httpClient.DefaultRequestHeaders.Range = new System.Net.Http.Headers.RangeHeaderValue(existingFileSize, null);

                    // ces 2 headers sont indipensables. Sans eux le téléchargement échoue
                    httpClient.DefaultRequestHeaders.Add("Accept","text/html, application/xhtml+xml, */*");
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");

                    // Téléchargement du contenu du fichier ZIP en tant qu'octets
                    using (HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            using (Stream responseStream = await response.Content.ReadAsStreamAsync())
                            using (FileStream fileStream = new FileStream(destinationPath, FileMode.Append))
                            {
                                byte[] buffer = new byte[10452643];
                                int bytesRead;
                                while ((bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                                }
                            }
                            Console.WriteLine("done!");
                            Console.WriteLine("");
                            Console.WriteLine("Extracting...");
                            try
                            {
                                ZipFile.ExtractToDirectory(destinationPath, phpVersionsFolder);
                                phpIni = Path.Combine(phpVersionsFolder, "php.ini-development");
                                if (File.Exists(phpIni)) {
                                    File.Move(phpIni, Path.Combine(phpVersionsFolder, "php.ini"));
                                }
                                Console.Write("done!");
                                Console.WriteLine("");
                                Console.WriteLine("Enter command: 'pvm use " + version + "' to activate that PHP version on your system.");
                                Console.WriteLine("");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("");
                                Console.WriteLine($"An Error occurred : {ex.Message}");
                                Console.WriteLine("");
                                return;
                            }

                        }
                        else
                        {
                            Console.WriteLine("");
                            Console.WriteLine($"Error HTTP : {response.StatusCode}");
                            Console.WriteLine("");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An Error occurred : {ex.Message}");
            }
        }
    }
}


