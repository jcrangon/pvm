using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using HtmlAgilityPack;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace PhpVersionManager
{
    internal class Web
    {
        

        public static void Harvester(string version = "") {
            // URL de la page à analyser
            string url = "https://windows.php.net/downloads/releases/archives/";
            var X86Nts = Array.Empty<string>();
            var X86Ts = Array.Empty<string>();
            var X64Nts = Array.Empty<string>();
            var X64Ts = Array.Empty<string>();

            // Créez une instance WebClient pour télécharger la page HTML
            HtmlWeb web = new();
            HtmlDocument document = web.Load(url);

            // XPath pour extraire les liens de téléchargement
            string xpath = "//a";

            // Sélectionnez tous les éléments <a> correspondants à l'expression XPath
            HtmlNodeCollection nodes = document.DocumentNode.SelectNodes(xpath);
            if (nodes != null)
            {
                Console.WriteLine("Quelle version majeure de PHP recherchez vous ? [5, 7, 8, ...] ? [Appuyez sur ENTER pour afficher toutes les versions disponibles] ");
                Console.Write("Votre choix....");
                string answer = "";
                var keyInfo = Console.ReadKey(intercept: true);
                answer += keyInfo.KeyChar;
                Console.WriteLine($"[{answer}]");

                // Utilisez LINQ pour filtrer les liens qui correspondent à vos critères
                var downloadNtsX86Links = nodes
                    .Where(node => node.GetAttributeValue("href", "").Contains("php-" + answer)
                                && node.GetAttributeValue("href", "").Contains("-nts")
                                && node.GetAttributeValue("href", "").Contains("x86")
                                && !node.GetAttributeValue("href", "").Contains("debug")
                                && !node.GetAttributeValue("href", "").Contains("devel")
                                && node.GetAttributeValue("href", "").EndsWith(".zip"))
                    .Select(node => node.GetAttributeValue("href", ""));

                var downloadTsX86Links = nodes
                    .Where(node => node.GetAttributeValue("href", "").Contains("php-" + answer)
                                && node.GetAttributeValue("href", "").Contains("x86")
                                && !node.GetAttributeValue("href", "").Contains("-nts")
                                && !node.GetAttributeValue("href", "").Contains("test")
                                && !node.GetAttributeValue("href", "").Contains("src")
                                && !node.GetAttributeValue("href", "").Contains("debug")
                                && !node.GetAttributeValue("href", "").Contains("devel")
                                && node.GetAttributeValue("href", "").EndsWith(".zip"))
                    .Select(node => node.GetAttributeValue("href", ""));

                var downloadNtsX64Links = nodes
                    .Where(node => node.GetAttributeValue("href", "").Contains("php-" + answer)
                                && node.GetAttributeValue("href", "").Contains("-nts")
                                && node.GetAttributeValue("href", "").Contains("x64")
                                && !node.GetAttributeValue("href", "").Contains("debug")
                                && !node.GetAttributeValue("href", "").Contains("devel")
                                && node.GetAttributeValue("href", "").EndsWith(".zip"))
                    .Select(node => node.GetAttributeValue("href", ""));

                var downloadTsX64Links = nodes
                    .Where(node => node.GetAttributeValue("href", "").Contains("php-" + answer)
                                && node.GetAttributeValue("href", "").Contains("x64")
                                && !node.GetAttributeValue("href", "").Contains("-nts")
                                && !node.GetAttributeValue("href", "").Contains("test")
                                && !node.GetAttributeValue("href", "").Contains("src")
                                && !node.GetAttributeValue("href", "").Contains("debug")
                                && !node.GetAttributeValue("href", "").Contains("devel")
                                && node.GetAttributeValue("href", "").EndsWith(".zip"))
                    .Select(node => node.GetAttributeValue("href", ""));

                
                foreach (var downloadLink in downloadNtsX86Links)
                {
                    Array.Resize(ref X86Nts, X86Nts.Length + 1);
                    X86Nts[X86Nts.GetUpperBound(0)] = downloadLink.Replace("/downloads/releases/archives/", "").Replace(".zip", "");
                    // Console.WriteLine(downloadLink.Replace("/downloads/releases/archives/", ""));
                }

                foreach (var downloadLink in downloadNtsX86Links)
                {
                    Array.Resize(ref X86Ts, X86Ts.Length + 1);
                    X86Ts[X86Ts.GetUpperBound(0)] = downloadLink.Replace("/downloads/releases/archives/", "").Replace(".zip", "");
                    // Console.WriteLine(downloadLink.Replace("/downloads/releases/archives/", ""));
                }
                
                foreach (var downloadLink in downloadNtsX64Links)
                {
                    Array.Resize(ref X64Nts, X64Nts.Length + 1);
                    X64Nts[X64Nts.GetUpperBound(0)] = downloadLink.Replace("/downloads/releases/archives/", "").Replace(".zip", "");
                    // Console.WriteLine(downloadLink.Replace("/downloads/releases/archives/", ""));
                }

                foreach (var downloadLink in downloadTsX64Links)
                {
                    Array.Resize(ref X64Ts, X64Ts.Length + 1);
                    X64Ts[X64Ts.GetUpperBound(0)] = downloadLink.Replace("/downloads/releases/archives/", "").Replace(".zip", "");
                    // Console.WriteLine(downloadLink.Replace("/downloads/releases/archives/", ""));
                }

                if(X86Nts.Length == 0 && X86Ts.Length == 0 && X64Nts.Length ==0 && X64Ts.Length ==0)
                {
                    Console.WriteLine("Aucun lien de téléchargement trouvé.");
                    return;
                }

                Console.WriteLine("");
                Console.WriteLine(" ----------------");
                Console.WriteLine("|  X86 (32bits)  |");
                Console.WriteLine(" ----------------");
                Utils.makeTables("NTS", "TS", X86Nts, X86Ts);

                Console.WriteLine("");
                Console.WriteLine(" ----------------");
                Console.WriteLine("|  X64 (64bits)  |");
                Console.WriteLine(" ----------------");
                Utils.makeTables("NTS", "TS", X64Nts, X64Ts);
            }
            else
            {
                Console.WriteLine("Aucun lien de téléchargement trouvé.");
            }


        }
    }
}
