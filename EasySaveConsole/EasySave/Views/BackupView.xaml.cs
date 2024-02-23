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
        private void DetailBackupClick(object sender, RoutedEventArgs e)
        {
            if (Backups.SelectedItem != null)
            {
                IBackup backup = (IBackup)Backups.SelectedItem;

                // Afficher les détails dans une MessageBox
                MessageBox.Show($"Name: {backup.Name}\nSource: {backup.Source}\nDestionation: {backup.Cible}\nType: {backup.Type}\nStatus: {backup.Status}", "Détails du Log");
            }
        }

        private void Backups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
