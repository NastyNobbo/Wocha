using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
    public partial class ChatWindow : Window
    {
        private string _userName;
        private TcpClient _client;
        private TcpListener _server;

        public ChatWindow(string userName, TcpClient client)
        {
            InitializeComponent();
            _userName = userName;
            chatTextBox.Text += ($"{_userName} подключился к чату."); // Сообщение о подключении
            _client = client;
            StartReceivingMessages();
        }

        public ChatWindow(string userName, TcpListener server)
        {
            InitializeComponent();
            _userName = userName;
            chatTextBox.Text += ($"{_userName} создал канал."); // Сообщение о создании канала
            _server = server;
            StartReceivingMessages();
        }

        private async void StartReceivingMessages()
        {
            if (_client != null)
            {
                NetworkStream stream = _client.GetStream();
                byte[] buffer = new byte[1024];

                try
                {
                    while (true)
                    {
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            // Добавляем сообщение в TextBox
                            Dispatcher.Invoke(() => AppendMessage(message));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при получении сообщения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (_server != null)
            {
                while (true)
                {
                    TcpClient client = await _server.AcceptTcpClientAsync();
                    _ = Task.Run(() => HandleClient(client));
                }
            }
        }

        private async Task HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            try
            {
                while (true)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        // Добавляем сообщение в TextBox
                        Dispatcher.Invoke(() => AppendMessage(message));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обработке клиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void messageTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true; // Предотвращаем звуковой сигнал при нажатии Enter
                SendMessage();
            }
        }

        private void SendMessage()
        {
            if (!string.IsNullOrWhiteSpace(messageTextBox.Text))
            {
                string message = $"{_userName}: {messageTextBox.Text}";
                try
                {
                    SendToServer(message);
                    AppendMessage(message);
                    messageTextBox.Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при отправке сообщения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Введите сообщение перед отправкой.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SendToServer(string message)
        {
                NetworkStream stream = _client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes(message);
                stream.Write(data, 0, data.Length);
            
        }

        private void AppendMessage(string message)
        {
            chatTextBox.Text += message + "\n";
            chatTextBox.ScrollToEnd(); // Прокручиваем вниз, чтобы видеть последнее сообщение
        }
    }

    
}
