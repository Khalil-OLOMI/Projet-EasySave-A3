using EasySaveConsole.Models;
using EasySaveConsole.Services;
using System;
using System.ComponentModel.Design;
using EasySaveConsole.Services;

namespace EasySaveConsole
{
    class Program
    {
        public static List<IBackup> backupList = new List<IBackup>();
        static void Main()
        {
            Menu();
        }

        static void Menu()
        {
            // Title of application
            Console.WriteLine("*********EasySave******");

            // Languages chosing if is null in sys config
            /*if (LanguageService.GetLanguage == null)
            {
                Console.WriteLine("Choisissez une langue - Choice a language");
                Console.WriteLine("1- Français");
                Console.WriteLine("2- Anglais");
                int language = Convert.ToInt32(Console.ReadLine());
                switch (language)
                {
                    case 1:
                        LanguageService.UpdateConfig("French");
                        break;
                    case 2:
                        LanguageService.UpdateConfig("English");
                        break;
                }
            }*/

            // Save choice in sys config
            //Menu d'utilisation
            Console.WriteLine("Que voulez-vous faire ?");
            Console.WriteLine("1- Ajouter un travail de sauvegarde");
            Console.WriteLine("2- Voir la liste des travaux de sauvegarde");
            Console.WriteLine("3- Exécuter un travail de sauvegarde");
            Console.WriteLine("4- Consulter les logs d'un travail de sauvegarde");
            Console.WriteLine("5- Historique des travaux de sauvegarde");

            int menu_choice = Convert.ToInt32(Console.ReadLine());

            switch (menu_choice)
            {
                case 1:
                    Console.WriteLine("Quelle type de sauvegarde voulez-vous créer ?");
                    Console.WriteLine("1- Complet");
                    Console.WriteLine("2- Differentiel");
                    int backup_choice = Convert.ToInt32(Console.ReadLine());
                    switch (backup_choice)
                    {
                        case 1:
                            BackupViewModel.AddBackup("Complet");
                            break;
                        case 2:
                            BackupViewModel.AddBackup("Differential");
                            break;
                    }
                    break;
                case 2:
                    BackupViewModel.GetBackupList();
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
            }
            Menu();
        }
    }
}
