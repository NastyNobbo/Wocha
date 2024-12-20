﻿using System;
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
        }

        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            if (loginTextBox.Text == "")
            {
                MessageBox.Show("Требуется ввести Логин", "Ошибка при вводе имени",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Вы создаёте канал?", "Выбор",
                    MessageBoxButton.YesNo);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        
                        CreatorChatWindow creatorWindow = new CreatorChatWindow(loginTextBox.Text);
                        creatorWindow.Show();
                        break;
                    case MessageBoxResult.No:
                        ClientChatWindow clientWindow = new ClientChatWindow(loginTextBox.Text);
                        clientWindow.Show();
                        break;
                }

                
                
                this.Close();
            }
        }
    }
}
