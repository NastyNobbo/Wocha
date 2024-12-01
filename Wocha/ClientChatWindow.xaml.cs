using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
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

namespace Wocha
{
    /// <summary>
    /// Логика взаимодействия для ChatWindow.xaml
    /// </summary>
    public partial class ClientChatWindow : Window
    {
        private string _userName;
        private TcpClient _client;

        public ClientChatWindow(string userName)
        {
            InitializeComponent();
            _userName = userName;
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            string serverIpAddress = ipTextBox.Text;
            if (!int.TryParse(portTextBox.Text, out int serverPort))
            {
                MessageBox.Show("Введите корректный номер порта", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _client = new TcpClient();
            try
            {
                _client.Connect(serverIpAddress, serverPort);
                ChatWindow chatWindow = new ChatWindow(_userName, _client);
                chatWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка подключения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
