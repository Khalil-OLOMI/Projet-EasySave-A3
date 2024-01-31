using System;

namespace EasySaveConsole
{
    class Program
    {
        static void Main()
        {
            // Title of application
            Console.WriteLine("*********EasySave******");

            // Languages chosing if is null in sys config
            Console.WriteLine("Choisissez une langue - Choice a language");
            Console.WriteLine("1- Français");
            Console.WriteLine("2- Anglais");
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
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
            }
        }
    }
}
