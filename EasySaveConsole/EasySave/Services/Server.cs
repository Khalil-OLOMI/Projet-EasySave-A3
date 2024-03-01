using EasySave.Models;
using EasySave.ViewModels;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Service
{
    public class Server
    {
        private TcpListener tcpListener;
        private bool isRunning;
        private BackupViewModel backupViewModel; // Ajoutez cette ligne

        public Server()
        {
            tcpListener = new TcpListener(IPAddress.Any, 12345);
            isRunning = false;
            backupViewModel = new BackupViewModel(); // Ajoutez cette ligne
        }

        public void Start()
        {
            if (!isRunning)
            {
                isRunning = true;
                tcpListener.Start();

                Task.Run(() => ListenForClients());
            }
        }

        public void Stop()
        {
            if (isRunning)
            {
                isRunning = false;
                tcpListener.Stop();
            }
        }

        private async Task ListenForClients()
        {
            try
            {
                while (isRunning)
                {
                    TcpClient client = await tcpListener.AcceptTcpClientAsync();
                    HandleClient(client);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error listening for clients: {ex.Message}");
            }
        }

        private async Task HandleClient(TcpClient tcpClient)
        {
            try
            {
                NetworkStream clientStream = tcpClient.GetStream();

                // Assume you have a method to serialize your backup list to JSON
                string backupListJson = SerializeBackupList();

                // Convert the backup list JSON to bytes
                byte[] backupListBytes = Encoding.UTF8.GetBytes(backupListJson);

                // Send the backup list to the client
                await clientStream.WriteAsync(backupListBytes, 0, backupListBytes.Length);

                // Close the connection
                tcpClient.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling client: {ex.Message}");
            }
        }

        private string SerializeBackupList()
        {
            BackupViewModel backupViewModel = new BackupViewModel();

            try
            {
                // Serialize the list of backups to JSON
                string backupListJson = JsonConvert.SerializeObject(backupViewModel.Backups, Formatting.Indented);
                return backupListJson;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error serializing backup list: {ex.Message}");
                return null; // Or handle the error in an appropriate way for your application
            }
        }
    }
}
