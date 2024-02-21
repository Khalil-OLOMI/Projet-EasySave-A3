using EasySave.Models;
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
            string type = Type.Text;

            viewModel.AddBackup(name, source, cible, type);
            Close(); 
        }
    }
}
