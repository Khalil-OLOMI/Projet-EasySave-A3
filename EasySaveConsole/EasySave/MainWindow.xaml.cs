using EasySave.Helpers;
using EasySave.ViewModels;
using EasySave.Views;
using System.ComponentModel;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private DeepLTranslator translator;
        public MainWindow()
        {
            InitializeComponent();
            string apiKey = Config.ApiKey;
            translator = new DeepLTranslator(apiKey);
            TranslateText();
            MainFrame.Content = new DashboardView();
        }
        private void NavDashbord(object sender, RoutedEventArgs e)
        {
            MainWindow newMainWindow = new MainWindow();
            newMainWindow.Show();
            Close();
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

        private async Task TranslateText()
        {
            DataContext = new
            {
                NameDashboard = await translator.TranslateAsync("Tableau de bord"),
                NameBackups = await translator.TranslateAsync("Travail de sauvegarde"),
                NameLogs = await translator.TranslateAsync("Journalisation"),
                NameSetting = await translator.TranslateAsync("Paramètres"),
                NameDocs = await translator.TranslateAsync("Documentation"),
            };
            OnPropertyChanged(nameof(DataContext));

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


    }
}