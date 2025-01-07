using Client.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Client.Services
{
    public class ComandService
    {
        private UI UIWindow;

        public ComandService(){}
        public ComandService(UI obj) {
            UIWindow = obj;
        }


        public async Task<Message> HandleComand(Message commandMessage)
        {
            switch (commandMessage.Comand)
            {
                case "Login":
                    return commandMessage;

                case "SignUp":
                    return commandMessage;

                case "Exiting":
                    return commandMessage;

                case "ListGroups":
                    return await List_Users_Groups(commandMessage, UIWindow.ListBox_Groups);

                case "ListUsers":
                    return await List_Users_Groups(commandMessage, UIWindow.ListBox_Users);

                case "History":
                    return await History(commandMessage);

                case "GetGroupHistory":
                    return await GetGroupHistory(commandMessage);

                case "Message":
                    return await UsersMessageHandler(commandMessage);

                case "GroupMessage":
                    return await GroupMessageHandler(commandMessage);

                case "UsersConection":
                    return commandMessage;

                default:
                    break;
            }
            return new Message();
        }
        public async Task<Message> GroupMessageHandler(Message message)
        {
            if (UIWindow.GroupLabel.Content.ToString() == message.SenderIP) 
                UIWindow.GroupChat_ListBox.Items.Add(message.Data);

            return message;
        }

        public async Task<Message> UsersMessageHandler(Message message)
        {
            if(message.SenderIP == UIWindow.UserLabel.Content.ToString())
            UIWindow.UsersChat_ListBox.Items.Add(message.Data);

            return message;
        }

        public async Task<Message> GetGroupHistory(Message message)
        {
            if (message.Comand == "GetGroupHistory")
            {
                List<string> receivedList = JsonConvert.DeserializeObject<List<string>>(message.Data);
                UIWindow.GroupChat_ListBox.Items.Clear();
                foreach (var item in receivedList)
                {
                    UIWindow.GroupChat_ListBox.Items.Add(item);
                }
            }
            else
            {
                UIWindow.GroupChat_ListBox.Items.Clear();
            }
            return message;
        }

        public async Task<Message> History(Message message)
        {
            if (message.Comand == "History")
            {
                List<string> receivedList = JsonConvert.DeserializeObject<List<string>>(message.Data);
                UIWindow.UsersChat_ListBox.Items.Clear();
                foreach (var item in receivedList)
                {
                    UIWindow.UsersChat_ListBox.Items.Add(item);
                }
            }
            else
            {
                UIWindow.UsersChat_ListBox.Items.Clear();
            }
            return message;
        }

        public async Task<Message> List_Users_Groups(Message message, ListBox listBox)
        {
            if (message == null || string.IsNullOrEmpty(message.Data) || message.Data == "[]")
            {
                listBox.Items.Clear();
                return message;
            }

            List<string> receivedList = JsonConvert.DeserializeObject<List<string>>(message.Data);
            var listBoxItems = listBox.Items.Cast<string>().ToList();

            if (!receivedList.SequenceEqual(listBoxItems))
            {
                listBox.Items.Clear();
                foreach (var item in receivedList) 
                {
                    listBox.Items.Add(item);
                }
            }
            else
                message.Data = "Update Interval";

            return message;
        }

        

    }
}
