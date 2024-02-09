using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace EasySaveConsole.Models
{
    internal class DifferentialBackup : IBackup
    {
        // Propriétés de la sauvegarde différentielle
        public string Name { get; set; } // Nom de la sauvegarde
        public string Source { get; set; } // Répertoire source
        public string Cible { get; set; } // Répertoire cible

        // Méthode pour effectuer une sauvegarde différentielle
        public void Copy(string Source, string Cible)
        {
            // Obtenir la liste des fichiers dans le répertoire source
            string[] sourceFiles = Directory.GetFiles(Source, "*", SearchOption.AllDirectories);

            // Parcourir chaque fichier dans le répertoire source
            foreach (string sourceFile in sourceFiles)
            {
                // Obtenir le chemin relatif du fichier
                string relativePath = sourceFile.Substring(Source.Length + 1);

                // Construire le chemin du fichier cible
                string targetFile = Path.Combine(Cible, relativePath);

                // Vérifier si le fichier source existe dans le répertoire cible
                if (File.Exists(targetFile))
                {
                    // Calculer les hashs pour les fichiers source et cible
                    string sourceHash = CalculateFileHash(sourceFile);
                    string targetHash = CalculateFileHash(targetFile);

                    // Si les hashs sont différents, copier le fichier
                    if (sourceHash != targetHash)
                    {
                        // Créer le répertoire s'il n'existe pas
                        Directory.CreateDirectory(Path.GetDirectoryName(targetFile));

                        // Copier le fichier
                        File.Copy(sourceFile, targetFile, true);
                    }
                }
                else
                {
                    // Créer le répertoire s'il n'existe pas
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFile));

                    // Copier le fichier
                    File.Copy(sourceFile, targetFile);
                }
            }
        }

        // Méthode pour calculer le hash d'un fichier
        private string CalculateFileHash(string filePath)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
