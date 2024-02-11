using EasySaveConsole.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveConsole.Models
{
    class CompletBackup : IBackup
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public string Cible { get; set; }

        public void Copy(string source, string cible)
        {
            DirectoryInfo dir = new DirectoryInfo(source);
            int nbre_file = 0;

            if (!dir.Exists)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Repertoire source inaccessible");
                Console.ResetColor();
            }
            else
            {
                if (!Directory.Exists(cible))
                {
                    Directory.CreateDirectory(cible);
                }
                foreach (string file in Directory.GetFiles(source))
                {
                    string targetFile = Path.Combine(cible, Path.GetFileName(file));
                    File.Copy(file, targetFile, true);
                    State state = new State()
                    {
                        Name = this.Name,
                        Horodatage = DateTime.Now,
                        Status = "ACTIVE",
                        FileSource = file,
                        FileTarget = targetFile,
                        TotalFilesToCopy = StateViewModel.FileNbre(source),
                        TotalFilesSize = StateViewModel.GetDirectorySize(source),
                        NbFilesLeftToDo = StateViewModel.FileNbre(source) - nbre_file,
                        Progression = ((double)(StateViewModel.FileNbre(source) - (StateViewModel.FileNbre(source) - nbre_file)) / StateViewModel.FileNbre(source)) * 100,
                    };
                    StateViewModel.WriteState(state);
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write(new string(' ', Console.WindowWidth - 1) + "\r");
                    Console.Write("Progression: " + nbre_file +"/" + state.TotalFilesToCopy);
                    nbre_file++;
                }
                foreach (string subdir in Directory.GetDirectories(Source))
                {
                    string targetSubDir = Path.Combine(cible, Path.GetFileName(subdir));
                    Copy(subdir, targetSubDir);
                }
            }
        }
    }
}
