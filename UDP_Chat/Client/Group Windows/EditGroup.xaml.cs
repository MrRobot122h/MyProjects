using Client.Models;
using Client.Services;
using Newtonsoft.Json;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Client.Group_Windows
{
    /// <summary>
    /// Interaction logic for EditGroup.xaml
    /// </summary>
    public partial class EditGroup : Window
    {
        private DispatcherTimer _timer;
        private ComandService comandService;
        private User user;
        private Message message;
        private ClientCode client;



        public EditGroup(User user)
        {
            InitializeComponent();

            comandService = new ComandService();
            client = ClientCode.GetInstance();

            this.user = user;

            StartTimer();
            //comandService.List_Users_Groups();

        }

        private void StartTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += async (sender, e) => await OnTimerTickAsync();
            _timer.Start();
        }

        private async Task OnTimerTickAsync()
        {
            await Task.WhenAll(ListUsers_NotInGroup(), ListGroups(), ListUsersInGroup());
        }

        private async Task ListUsersInGroup()
        {
            if (Groups_ListBox.SelectedItem != null)
            {
                string GroupName = Groups_ListBox.SelectedItem.ToString();
                message = new Message("ListUsersInGroup", GroupName, null, null);
                await ClientCode.SendMessage(message);
                message = await client.JustReceive();

                if (message != null && message.Comand == "ListUsersInGroup")
                {
                    await comandService.List_Users_Groups(message, this.UsersInGroup_ListBox);
                }
            }
            else
            {
                UsersInGroup_ListBox.Items.Clear();
            }
        }

        private async Task ListUsers_NotInGroup() 
        {
            if (Groups_ListBox.SelectedItem != null)
            {
                string GroupName = Groups_ListBox.SelectedItem.ToString();
                message = new Message("ListUsersNotInGroup", GroupName, null, null);
                await ClientCode.SendMessage(message);
                message = await client.JustReceive();
                if (message != null && message.Comand == "ListUsersNotInGroup")
                {
                    await comandService.List_Users_Groups(message, this.Users_ListBox);
                }
            }
            else
            {
                Users_ListBox.Items.Clear();
            }
        }

        private async Task ListGroups()
        {
            string Json = JsonConvert.SerializeObject(user);
            message = new Message("ListGroups", Json, null, null);
            await ClientCode.SendMessage(message);
            message = await client.JustReceive();
            if(message.Comand == "ListGroups")
            {
                await comandService.List_Users_Groups(message, this.Groups_ListBox);
            }

        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (Groups_ListBox.SelectedItem != null && Users_ListBox.SelectedItem != null)
            {
                string GroupName = Groups_ListBox.SelectedItem.ToString();
                string UserName = Users_ListBox.SelectedItem.ToString();
                message = new Message("AddToGroup", GroupName, null, UserName);
                await ClientCode.SendMessage(message);

            }

        }

        private async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Groups_ListBox.SelectedItem != null && UsersInGroup_ListBox.SelectedItem != null)
            {
                string GroupName = Groups_ListBox.SelectedItem.ToString();
                string UserName = UsersInGroup_ListBox.SelectedItem.ToString();
                message = new Message("DeleteFromGroup", GroupName, null, UserName);
                await ClientCode.SendMessage(message);
            }
        }

        private async void ChangeName_Button_Click(object sender, RoutedEventArgs e)
        {
            if (newName_TextBox.Text.Length > 2 && Groups_ListBox.SelectedItem != null)
            {
                string OldName = Groups_ListBox.SelectedItem.ToString();
                message = new Message("Change group name", newName_TextBox.Text, null, OldName);
                newName_TextBox.Text = string.Empty;
                await ClientCode.SendMessage(message);
            }
        }
    }
}
