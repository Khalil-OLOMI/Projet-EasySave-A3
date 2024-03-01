using EasySave.Models;
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
    /// Logique d'interaction pour ProgressView.xaml
    /// </summary>
    public partial class ProgressView : Page
    {
        private IBackup _selectedBackup;
        public ProgressView(IBackup selectedBackup)
        {
            InitializeComponent();

            _selectedBackup = selectedBackup;

            // Initialisez la page avec le modèle de sauvegarde actuel
            DataContext = _selectedBackup;
        }

        private void BackToBackupsClick(object sender, RoutedEventArgs e)
        {
            // Assuming you have a Frame named "MainFrame" in your main window
            // You can replace "MainFrame" with the actual name of your Frame control
            Frame frame = Application.Current.MainWindow.FindName("MainFrame") as Frame;

            if (frame != null)
            {
                // Navigate back to the BackupsPage
                frame.GoBack();
            }
        }
    }
}
