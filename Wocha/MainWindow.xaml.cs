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

namespace Wocha
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            loginTextBox.Focus();
        }

        private void CreatorButton_Click(object sender, RoutedEventArgs e)
        {
            if (loginTextBox.Text == "")
            {
                MessageBox.Show("Требуется ввести Логин", "Ошибка при вводе имени",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                CreatorChatWindow creatorWindow = new CreatorChatWindow(loginTextBox.Text);
                creatorWindow.Show();
                this.Close();
            }

        }
        private void ClientButton_Click(object sender, RoutedEventArgs e)
        {
            if (loginTextBox.Text == "")
            {
                MessageBox.Show("Требуется ввести Логин", "Ошибка при вводе имени",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                ClientChatWindow creatorWindow = new ClientChatWindow(loginTextBox.Text);
                creatorWindow.Show();
                this.Close();
            }

        }
    }
}
