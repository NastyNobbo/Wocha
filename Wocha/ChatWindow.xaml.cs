using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Wocha
{
    public partial class ChatWindow : Window
    {
        private string _userName;
        private TcpClient _client;
        private TcpListener _server;
        private static List<TcpClient> _clients = new List<TcpClient>();

        public ChatWindow(string userName, TcpClient client)
        {
            InitializeComponent();
            _userName = userName;
            _client = client;
            _clients.Add(client); // Добавляем клиента в список
           
            StartReceivingMessages();
        }

        public ChatWindow(string userName, TcpListener server)
        {
            InitializeComponent();

            _userName = userName;
            chatTextBox.Text += ($"{_userName} создал канал.\n");
            _server = server;
            StartReceivingMessages();
        }

        private async void StartReceivingMessages()
        {
            if (_client != null)
            {
                // Отправляем имя пользователя серверу при подключении
                NetworkStream stream = _client.GetStream();
                byte[] userNameBytes = Encoding.UTF8.GetBytes(_userName);
                await stream.WriteAsync(userNameBytes, 0, userNameBytes.Length);

                // Получение сообщений от клиента
                byte[] buffer = new byte[1024];

                try
                {
                    while (true)
                    {
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            Dispatcher.Invoke(() => AppendMessage(message));
                        }
                    }
                }
                catch (IOException ioEx)
                {

                    MessageBox.Show($"Соединение закрыто: {ioEx.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при получении сообщения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (_server != null)
            {
                // Ожидание подключения клиентов
                while (true)
                {
                    TcpClient client = await _server.AcceptTcpClientAsync();
                    _clients.Add(client); // Добавляем нового клиента в список
                    _ = Task.Run(() => HandleClient(client, _userName)); // Передаем имя пользователя сервера
                }
            }
        }

        private async Task HandleClient(TcpClient client, string serverUsername)
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];

            try
            {
                // Получение имени пользователя клиента
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string clientUsername = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                
                // Отправляем имя пользователя серверу
                string connectionMessage = $"{clientUsername} подключился к чату.";
                SendToAllClients(connectionMessage);
                Dispatcher.Invoke(() => AppendMessage(connectionMessage));

                while (true)
                {
                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Dispatcher.Invoke(() => AppendMessage(message));
                        BroadcastMessage(message, client); // Рассылаем сообщение всем клиентам, кроме отправителя
                    }
                }
            }
            catch (IOException ioEx)
            {
                MessageBox.Show($"Соединение закрыто: {ioEx.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обработке клиента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Удаляем клиента из списка при отключении
                _clients.Remove(client);
                client.Close();
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
                e.Handled = true; 
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
                    // Проверка состояния соединения перед отправкой сообщения
                    
                        SendToAllClients(message); // Отправляем сообщение всем клиентам
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

        private void SendToAllClients(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            foreach (var client in _clients)
            {
                if (client.Connected)
                {
                    NetworkStream stream = client.GetStream();
                    stream.Write(data, 0, data.Length);
                }
            }
        }

        private void AppendMessage(string message)
        {
            chatTextBox.Text += $"{message}\n";
            chatTextBox.ScrollToEnd(); // Прокрутка к последнему сообщению
        }

        private void BroadcastMessage(string message, TcpClient sender)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            foreach (var client in _clients)
            {
                if (client != sender && client.Connected) // Не отправляем сообщение отправителю
                {
                    NetworkStream stream = client.GetStream();
                    stream.Write(data, 0, data.Length);
                }
            }
        }
    }
}