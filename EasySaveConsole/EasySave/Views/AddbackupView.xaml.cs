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
using System.Windows.Shapes;

namespace EasySave.Views
{
    /// <summary>
    /// Logique d'interaction pour AddbackupView.xaml
    /// </summary>
    public partial class AddbackupView : Window
    {
        private BackupViewModel viewModel;

        public AddbackupView(BackupViewModel backupViewModel)
        {
            InitializeComponent();
            viewModel = backupViewModel;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string name = Name.Text;
            string source = Source.Text;
            string cible = Cible.Text;
            bool isComplete = CompletRadioButton.IsChecked ?? false;
            bool isDifferential = DiffrentiellleRadioButton.IsChecked ?? false;

            viewModel.AddBackup(name, source, cible, isComplete, isDifferential);
            Close();
            
        }
        private void BrowseSource_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.ValidateNames = false;
            openFileDialog.CheckFileExists = false;
            openFileDialog.CheckPathExists = true;
            openFileDialog.FileName = "Folder Selection";
            openFileDialog.Filter = "Folder|*.thisdoesnotexist";
            if (openFileDialog.ShowDialog() == true)
            {
                Source.Text = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
            }
        }

        private void BrowseTarget_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.ValidateNames = false;
            openFileDialog.CheckFileExists = false;
            openFileDialog.CheckPathExists = true;
            openFileDialog.FileName = "Folder Selection";
            openFileDialog.Filter = "Folder|*.thisdoesnotexist";
            if (openFileDialog.ShowDialog() == true)
            {
                Cible.Text = System.IO.Path.GetDirectoryName(openFileDialog.FileName);
            }
        }

    }
}
