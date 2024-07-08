using Newtonsoft.Json;
using System.Net;
using System.Windows;
using System.Windows.Media.TextFormatting;
using UDPClient;

namespace UDPServer
{
    public class ComandsHandler
    {
        private CurrentUser currentUser;
        public List<CurrentUser> ListUsers;
        private Chat ChatWindow;
        public ComandsHandler(Chat obj)
        {
            this.ChatWindow = obj;
            currentUser = new CurrentUser();
        }

        public ComandsHandler()
        {
            currentUser = new CurrentUser();
        }

        public CommandMessage HandleComand(CommandMessage commandMessage)
        {
            switch(commandMessage.Comand)
            {
                case "Login":
                    return commandMessage;

                case "SignUp":
                    return commandMessage;

                case "Exiting":
                    return commandMessage;

                case "ListUsers":
                    return List(commandMessage);

                case "Message":
                    Message(commandMessage);
                    return commandMessage;

                case "History":
                    History(commandMessage);
                    return commandMessage;

                default:
                    break;
            }
            return null;
        }
        public CommandMessage List(CommandMessage commandMessage)
        {  
            ListUsers = JsonConvert.DeserializeObject<List<CurrentUser>>(commandMessage.Data);
            if (ChatWindow != null && ChatWindow.UsersList != null)
            {
                ChatWindow.Dispatcher.Invoke(() =>
                {
                    ChatWindow.UsersList.ItemsSource = ListUsers;
                });
            }
            return commandMessage;
        }
        public void Message(CommandMessage commandMessage)
        {
            if(commandMessage != null && ChatWindow != null 
             && ChatWindow.UsersList != null && 
             ChatWindow.LableName.Content.ToString() == commandMessage.Sender)
            {
                string massage = $"{commandMessage.Sender}: {commandMessage.Data}\n";

                ChatWindow.Dispatcher.Invoke(() =>
                {
                    ChatWindow.ChatlineTB.Text += massage;
                });
            }
        }
        public void History(CommandMessage commandMessage)
        { 
            if (commandMessage != null && ChatWindow != null && ChatWindow.UsersList != null)
            {
                string massage = $"{commandMessage.Data}\n";

                ChatWindow.Dispatcher.Invoke(() =>
                {
                    ChatWindow.ChatlineTB.Text += massage;
                });
            }
        }
    }
}
