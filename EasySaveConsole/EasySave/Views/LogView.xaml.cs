using EasySave.Models;
using EasySave.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace EasySave.Views
{
    /// <summary>
    /// Logique d'interaction pour LogView.xaml
    /// </summary>
    public partial class LogView : Page
    {
        public LogView()
        {
            InitializeComponent();
            DataContext = new LogViewModel();
        }

        private void DetailLogClick(object sender, RoutedEventArgs e)
        {
            if (Logs.SelectedItem != null)
            {
                try
                {
                    Log log = (Log)Logs.SelectedItem;

                    // Afficher les détails dans une MessageBox
                    MessageBox.Show($"Date: {log.Horodatage}\nName: {log.Name}\nSource: {log.FileSource}\nDestionation: {log.FileTarget}\nFileSize: {log.FileSize}\nFileTransferTime: {log.FileTransferTime}", "Détails du Log");
                }
                catch
                {
                    MessageBox.Show("Vide");
                }
            }
        }
    }
}
