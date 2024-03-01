using EasySaveClient.Models;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace EasySaveClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private TcpClient tcpClient;
        private NetworkStream clientStream;
        private ObservableCollection<Backup> backups;

        public MainWindow()
        {
            InitializeComponent();
            backups = new ObservableCollection<Backup>();
            InitializeClient();
            
            Backups.ItemsSource = backups;
        }

        private void InitializeClient()
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect("192.168.32.1", 12345); // Remplacez 127.0.0.1 par l'adresse IP du serveur

                clientStream = tcpClient.GetStream();

                // Démarrer une tâche pour écouter les données du serveur en arrière-plan
                Task.Run(() => ListenForServerData());
            } catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la connexion au serveur : {ex.Message}");
            }
        }

        private async Task ListenForServerData()
        {
            try
            {
                byte[] buffer = new byte[4096];
                int bytesRead;

                while (true)
                {
                    bytesRead = await clientStream.ReadAsync(buffer, 0, buffer.Length);

                    if (bytesRead == 0)
                        break;

                    string serverData = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // Désérialisez les données JSON reçues en objets de liste de sauvegardes
                    ObservableCollection<Backup> receivedBackups = DeserializeBackupList(serverData);

                    foreach (var receivedBackup in receivedBackups)
                    {
                        backups.Add(receivedBackup);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving server data: {ex.Message}");
            }
        }

        private ObservableCollection<Backup> DeserializeBackupList(string json)
        {
            try
            {
                // Désérialisez la chaîne JSON en une liste d'objets de sauvegardes
                return JsonConvert.DeserializeObject<ObservableCollection<Backup>>(json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializing backup list: {ex.Message}");
                return null; // Ou gérez l'erreur d'une manière appropriée pour votre application
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}