using EasySave.Models;
using EasySave.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace EasySave.Views
{
    public partial class ConfigView : Page
    {
        private ConfigViewModel _viewModel;

        public ConfigView()
        {
            InitializeComponent();
            InitializeViewModel();
        }

        private async void InitializeViewModel()
        {
            // Create an instance of ConfigViewModel and load config
            var config = Config.LoadConfig();
            _viewModel = new ConfigViewModel(config);

            // Set the DataContext of the view to the ViewModel
            DataContext = _viewModel;

            // Translate text when the page is loaded
            await _viewModel.TranslateText();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DataGrid_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
