using EasySave.Services;
using System.Configuration;
using System.Data;
using System.Windows;

namespace EasySave
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            //Init backup save file
            new BackupViewModel().InitBackup();
        }
    }

}
