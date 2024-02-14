using EasySaveConsole.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveConsole.Models
{
    // JB: ajouter public
    class CompletBackup : IBackup
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public string Cible { get; set; }
        

        // JB: Ici on a deux responsabilités qu'on pourrait séparer:
        // D'un coté le modèle avec les propriétés et de l'autre la classe permettant de créer une sauvegarde
        public void Copy(string source, string cible)
        {
            int nbre_file = 0;
            DirectoryInfo dir = new DirectoryInfo(source);

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
                    try
                    {
                        Directory.CreateDirectory(cible);
                    }
                    catch (Exception ex)
                    {
                        // Handle any exceptions that occur during directory creation
                        Console.WriteLine("An error occurred while creating the directory:");
                        Console.WriteLine(ex.Message);
                    }
                }

                foreach (string file in Directory.GetFiles(source))
                {
                    string targetFile = Path.Combine(cible, Path.GetFileName(file));
                    File.Copy(file, targetFile, true);
                    nbre_file++;
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
                    Console.Write("Progression: " + nbre_file + "/" + state.TotalFilesToCopy);
                    
                }
                foreach (string subdir in Directory.GetDirectories(source))
                {
                    string targetSubDir = Path.Combine(cible, Path.GetFileName(subdir));
                    Copy(subdir, targetSubDir);
                }
            }
        }
    }
}

