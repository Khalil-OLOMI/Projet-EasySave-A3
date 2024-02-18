using EasySave.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace EasySave.Models
{
    // JB: ajouter public
    class CompletBackup : IBackup
    {
        public string Name { get; set; }
        public string Source { get; set; }
        public string Cible { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }

        // JB: Ici on a deux responsabilités qu'on pourrait séparer:
        // D'un coté le modèle avec les propriétés et de l'autre la classe permettant de créer une sauvegarde
        public void Copy(string source, string cible)
        {
            int nbre_file = 0;
            DirectoryInfo dir = new DirectoryInfo(source);

            if (!dir.Exists)
            {
                MessageBox.Show("Dossier source inexistant");
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
                        MessageBox.Show("Erreur de création de dossier cible");
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
                    new StateViewModel().WriteState(state);

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

