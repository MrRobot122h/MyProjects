using Newtonsoft.Json;
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
using UDPServer;

namespace UDPClient
{
    /// <summary>
    /// Interaction logic for Chat.xaml
    /// </summary>
    public partial class Chat : Window
    {
        private Client client;
        private CommandMessage message;
        private CurrentUser user;
        private ComandsHandler commandsHandler;

        public Chat(CurrentUser obj)
        {
            InitializeComponent();
            client = Client.GetInstance();
            Closing += Chat_Closing;
            commandsHandler = new ComandsHandler(this);
            user = obj;
            this.Title = user.UserName;
            message = new CommandMessage
            {
                Comand = "ListUsers",
                Data = JsonConvert.SerializeObject(user)
            };
            Client.SendMessage(message);
            StartListening();
        }

        private void Chat_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            message = new CommandMessage("Exiting", this.Title,null,null);
            Client.SendMessage(message);
        }

        private void StartListening()
        { 
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var receivedMessage = await client.Receive();
                        if (receivedMessage != null)
                        {
                            Dispatcher.Invoke(() =>
                            {
                                commandsHandler.HandleComand(receivedMessage);
                            });
                        }
                    }
                    catch (SocketException ex)
                    {
                        MessageBox.Show($"Socket error while receiving: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Unexpected error while receiving: {ex.Message}");
                    }
                }
            });
        }

        private void UsersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChatlineTB.Clear();
            if (UsersList.SelectedItem is CurrentUser selectedUser)
            {
                LableName.Content = selectedUser.UserName;
                message = new CommandMessage
                {
                    Comand = "History",
                    Reciver = selectedUser.UserName,
                    Sender = this.Title
                };
                Client.SendMessage(message);
            }
        }

        private void SendMessageTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && LableName.Content as string != "" && SendMessageTB.Text != "")
            {
                ChatlineTB.Text += $"{this.Title}: {SendMessageTB.Text}\n";
                message = new CommandMessage("Message", SendMessageTB.Text, this.Title, LableName.Content.ToString());
                Client.SendMessage(message);
                SendMessageTB.Clear();
            }
        }
    }
}
