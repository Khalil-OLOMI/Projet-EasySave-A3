using EasySave.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EasySave
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Content = new DashboardView();
        }
        private void NavDashbord(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new DashboardView();
        }
        private void NavBackups(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new BackupView();
        }
        private void NavLogs(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new LogView();
        }
        private void NavSettings(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new ConfigView();
        }
        private void NavDocs(object sender, RoutedEventArgs e)
        {
            MainFrame.Content = new DocView();
        }

    }
}