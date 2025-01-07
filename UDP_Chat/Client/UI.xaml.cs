using Client.Group_Windows;
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

namespace Client
{
    /// <summary>
    /// Interaction logic for UI.xaml
    /// </summary>
    public partial class UI : Window
    {
        private DispatcherTimer _timer;
        private User thisUser;
        private Message message;
        private ClientCode client;
        private ComandService comandService;

        private CreateGroup createGroup;
        private DeleteGroup deleteGroup;
        private EditGroup editGroup;




        public UI(User user)
        {
            InitializeComponent();

            thisUser = user;
            this.Title = user.Login;
            client = ClientCode.GetInstance();
            client.ChangeComandService(this);
            Closing += UI_Closing;
            Task.Delay(1000);
            StartTimer();
            StartMessageListener();

        }

        private async void UI_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            string Json = JsonConvert.SerializeObject(thisUser);
            message = new Message("Exiting", Json, null, null);
            await ClientCode.SendMessage(message);
            message = await client.Receive();
            if (message.Data == "Exit")
            {
                this.Close();
            }
            else
            {
                MessageBox.Show(message.Data);
            }
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
            await Task.WhenAll(ListUsers(), ListGroups(), UpdateGroupChat());
        }

        private async Task StartMessageListener()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                      await client.Receive();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{ex.Message}");
                    }
                    await Task.Delay(new Random().Next(100, 500)); 
                }
            });
        }

        private async Task UpdateGroupChat()
        {
            message = new Message("GetGroupHistory", GroupLabel.Content.ToString(), null, null);
            await ClientCode.SendMessage(message);
            await client.Receive();
        }
        private async Task ListUsers()
        {
            string Json = JsonConvert.SerializeObject(thisUser);
            message = new Message("ListUsers", Json, null, null);
            await ClientCode.SendMessage(message);
            message = await client.Receive();

            if (message != null && message.Data == "Update Interval")
                _timer.Interval = TimeSpan.FromSeconds(new Random().Next(2, 5));
            else
                _timer.Interval = TimeSpan.FromSeconds(1);


        }

        private async Task ListGroups()
        {
            string Json = JsonConvert.SerializeObject(thisUser);
            message = new Message("ListGroups", Json, null, null);
            await ClientCode.SendMessage(message);
            message = await client.Receive();

            if (message != null && message.Data == "Update Interval")
                _timer.Interval = TimeSpan.FromSeconds(new Random().Next(2, 5));
            else
                _timer.Interval = TimeSpan.FromSeconds(1);
        }

        private async void ListBox_Groups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBox_Groups.SelectedItem is string GroupName)
            {
                GroupLabel.Content = GroupName;
                GroupChat_ListBox.Items.Clear();
                GroupInputTextBox.Text = string.Empty;

                message = new Message("GetGroupHistory", GroupName, null, null);
                await ClientCode.SendMessage(message);
                await client.Receive();

            }
        }

        private async void ListBox_Users_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBox_Users.SelectedItem is string Name)
            {
                UserLabel.Content = Name;
                UsersChat_ListBox.Items.Clear();
                UserInputTextBox.Text = string.Empty;

                List<string> Users = new List<string>
                {
                    UserLabel.Content?.ToString(),
                    thisUser.Login
                };

                if (Users[0] != null) 
                {
                    string JsonList = JsonConvert.SerializeObject(Users);
                    message = new Message("UsersConection", JsonList, null, null);
                    await ClientCode.SendMessage(message);
                    await client.Receive();
                }
            }
        }

        private async void UserInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(UserInputTextBox.Text) && UserLabel.Content.ToString() != "User:")
            {
                string messageText = $"{thisUser.Login}: {UserInputTextBox.Text}";
                UsersChat_ListBox.Items.Add(messageText);
                UserInputTextBox.Text = string.Empty;

                message = new Message("Message", messageText, null, UserLabel.Content.ToString());
                await ClientCode.SendMessage(message);
            }
        }

        private async void GroupInputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(GroupInputTextBox.Text) && GroupLabel.Content.ToString() != "Group:")
            {
                string messageText = $"{thisUser.Login}: {GroupInputTextBox.Text}";
                GroupChat_ListBox.Items.Add(messageText);
                GroupInputTextBox.Text = string.Empty;
                message = new Message("GroupMessage", messageText, null, GroupLabel.Content.ToString());
                await ClientCode.SendMessage(message);
            }
        }

        private async void ListBox_Users_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListBox_Users.SelectedItem != null)
            {
                if (ListBox_Users.SelectedItem is string Name)
                {
                    UserLabel.Content = Name;
                    UsersChat_ListBox.Items.Clear();
                    UserInputTextBox.Text = string.Empty;

                }
                List<string> Users = new List<string>();
                Users.Add(UserLabel.Content.ToString());
                Users.Add(thisUser.Login);

                string JsonList = JsonConvert.SerializeObject(Users);
                message = new Message("UsersConection", JsonList, null, null);
                await ClientCode.SendMessage(message);
                await client.Receive();
            }
        }

        private void Create_Button_Click(object sender, RoutedEventArgs e)
        {
            _timer.Interval = TimeSpan.FromSeconds(5);
            createGroup = new CreateGroup(thisUser);
            createGroup.Show();
            _timer.Interval = TimeSpan.FromSeconds(1);

        }

        private void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            _timer.Interval = TimeSpan.FromSeconds(5);
            deleteGroup = new DeleteGroup(thisUser);
            deleteGroup.Show();
            _timer.Interval = TimeSpan.FromSeconds(1);

        }

        private void Edit_Button_Click(object sender, RoutedEventArgs e)
        {

            _timer.Interval = TimeSpan.FromSeconds(5);
            editGroup = new EditGroup(thisUser);
            editGroup.Show();
            _timer.Interval = TimeSpan.FromSeconds(1);

        }


    }
}
