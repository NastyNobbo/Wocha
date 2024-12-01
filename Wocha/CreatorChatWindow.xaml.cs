using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
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
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;

namespace Wocha
{
    /// <summary>
    /// Логика взаимодействия для CreatorChatWindow.xaml
    /// </summary>
    public partial class CreatorChatWindow : Window
    {
        private string _userName;
        private TcpListener _server;

        public CreatorChatWindow(string userName)
        {
            InitializeComponent();
            PortFinder();
            IPAddress[] localIP = Dns.GetHostAddresses(Dns.GetHostName());

            foreach (IPAddress address in localIP)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipTextBox.Text = address.ToString();

                }
            }
            _userName = userName;
        }
        private void PortFinder()
        {
            Random random = new Random();
            int randomPort = 0;
            bool portFound = false;

            // Генерируем случайный порт, пока не найдем открытый
            while (!portFound)
            {
                randomPort = random.Next(49152, 65535); // Генерируем порт в диапазоне от 49152 до 65535
                if (IsPortOpen(randomPort))
                {
                    portFound = true;
                }

            }

            portTextBox.Text = randomPort.ToString();
        }

        private bool IsPortOpen(int port)
        {
            try
            {
                using (TcpListener listener = new TcpListener(IPAddress.Any, port))
                {
                    listener.Start();
                    listener.Stop();
                    return true; // Порт открыт
                }
            }
            catch (SocketException)
            {
                return false; // Порт закрыт
            }
        }
        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            string ipAddress = ipTextBox.Text;
            
            if (!int.TryParse(portTextBox.Text, out int port))
            {
                MessageBox.Show("Введите корректный номер порта", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                IPAddress ip = IPAddress.Parse(ipAddress);
                
            }
            catch (Exception ex)
            {
                this.Close(); // Закрываем окно конфигурации
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            _server = new TcpListener(IPAddress.Parse(ipAddress), port);
            _server.Start();
            _server.BeginAcceptTcpClient(new AsyncCallback(OnClientConnect), null);

            ChatWindow chatWindow = new ChatWindow(_userName, _server);
            chatWindow.Show();
            this.Close();
        }
        
        

        private void OnClientConnect(IAsyncResult ar)
        {
            TcpClient client = _server.EndAcceptTcpClient(ar);
            // Здесь можно создать поток для обработки сообщений
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            mainWindow.loginTextBox.Text = _userName;
            mainWindow.loginTextBox.Focus();
            this.Close();
        }
    }
}
