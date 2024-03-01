using EasySave.Services;
using EasySave.ViewModels;
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
        private static Mutex mutex = new Mutex(true, "{7A171F02-8501-4C33-9342-DA04B34798EE}");
        protected override void OnStartup(StartupEventArgs e)
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                //base.OnStartup(e);
                new BackupViewModel().InitBackup();
            }
            else
            {
                MessageBox.Show("L'application est déjà en cours d'exécution.");
                Current.Shutdown();
            }
        }
    }

}
