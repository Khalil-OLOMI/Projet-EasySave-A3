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
    /// Logique d'interaction pour BackupView.xaml
    /// </summary>
    public partial class BackupView : Page
    {
        public BackupView()
        {
            InitializeComponent();
            DataContext = new BackupViewModel();
        }

        private void AddBackupForm(object sender, RoutedEventArgs e)
        {
            AddbackupView addBackupView = new AddbackupView((BackupViewModel)DataContext);
            addBackupView.ShowDialog();
        }
        //private void AddBackupForm(object sender, RoutedEventArgs e)
        //{
        //    // Open the AddbackupView window
        //    AddbackupView addForm = new AddbackupView();
        //    addForm.ShowDialog(); // Use Show() for non-modal window
        //}

    }
}
