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
    /// Logique d'interaction pour DashboardView.xaml
    /// </summary>
    public partial class DashboardView : Page
    {
        public DashboardView()
        {
            InitializeComponent();
        }

        private void AddBackupClick(object sender, RoutedEventArgs e)
        {
            AddbackupView addBackupView = new AddbackupView((BackupViewModel)DataContext);
            addBackupView.ShowDialog();
        }

        private void BackupViewClick(object sender, RoutedEventArgs e)
        {
            Frame frame = Application.Current.MainWindow.FindName("MainFrame") as Frame;

            if (frame != null)
            {
                // Naviguer vers la page de progression en passant la sauvegarde en cours (selectedBackup)
                frame.Content = new BackupView();
            }
        }
    }
}
