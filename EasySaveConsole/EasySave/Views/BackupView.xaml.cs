using EasySave.Models;
using EasySave.Services;
using EasySave.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasySave.Views
{
    /// <summary>
    /// Logique d'interaction pour BackupView.xaml
    /// </summary>
    public partial class BackupView : Page
    {
        public BackupView()
        {
            DataContext = new BackupViewModel();
            InitializeComponent();
        }

        private void AddBackupForm(object sender, RoutedEventArgs e)
        {
            AddbackupView addBackupView = new AddbackupView((BackupViewModel)DataContext);
            addBackupView.ShowDialog();
        }
        private void DetailBackupClick(object sender, MouseButtonEventArgs e)
        {
            if (Backups.SelectedItem != null)
            {
                IBackup backup = (IBackup)Backups.SelectedItem;

                // Afficher les détails dans une MessageBox
                MessageBox.Show($"Name: {backup.Name}\nSource: {backup.Source}\nDestionation: {backup.Cible}\nType: {backup.Type}\nStatus: {backup.Status}", "Détails du Log");
            }
        }

        private void StateViewClick(object sender, MouseButtonEventArgs e)
        {
            // Obtenez la sauvegarde sur laquelle le clic a été effectué
            IBackup selectedBackup = (sender as DataGridCell)?.DataContext as IBackup;

            if (selectedBackup != null)
            {
                // Implémentez la logique pour naviguer vers la page de progression (par exemple, en changeant le contenu de votre Frame)
                Frame frame = Application.Current.MainWindow.FindName("MainFrame") as Frame;

                if (frame != null)
                {
                    // Naviguer vers la page de progression en passant la sauvegarde en cours (selectedBackup)
                    frame.Content = new ProgressView(selectedBackup);
                }
            }
        }
    }
}
