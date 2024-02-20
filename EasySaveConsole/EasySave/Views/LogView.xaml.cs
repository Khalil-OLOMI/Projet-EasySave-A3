using EasySave.Models;
using EasySave.Services;
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
