using Newtonsoft.Json;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using UDPServer;

namespace UDPClient
{
    public partial class MainWindow : Window
    {
        public Client client;
        CommandMessage message = new CommandMessage();
        DatePicker? datePicker;
        CurrentUser currentUser;
        Chat chatForm;
        public MainWindow()
        {
            InitializeComponent();
            client = Client.GetInstance();
        }
        private void SignUpRB_Checked(object sender, RoutedEventArgs e)
        {
            if(SignUpRB.IsChecked == true)
            {
                datePicker = new DatePicker()
                {
                    Name = "BirthDateDP",
                    Width = 200,
                    Margin = new Thickness(10, 10, 10, 10),
                    FontSize = 25
                };
                Label birthDateLabel = new Label()
                {
                    Content = "Birth Date:",
                    HorizontalAlignment = HorizontalAlignment.Left,
                    FontSize = 25,
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                
                Place1.Children.Add(datePicker);
                Place1.Children.Add(birthDateLabel);
                message.Comand = "SignUp";
            }
        }

        private void LoginRB_Checked(object sender, RoutedEventArgs e)
        {
            if (LoginRB.IsChecked == true)
            {
                Place1.Children.Clear();
                message.Comand = "Login";
            }
        }
        private async void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            string birthDate = datePicker?.SelectedDate?.ToString("yyyy-MM-dd");
            if (message.Comand == "Login")
                birthDate = DateTime.Now.ToShortDateString();

            currentUser = new CurrentUser(LoginTB.Text, PasswordTB.Text, birthDate, 1, null);
            if (!string.IsNullOrEmpty(currentUser.UserName)
                && !string.IsNullOrEmpty(currentUser.Password)
                && !string.IsNullOrEmpty(currentUser.BirthDate)
                && !string.IsNullOrEmpty(message.Comand))
            {
                message.Data = JsonConvert.SerializeObject(currentUser);
                Client.SendMessage(message);
                message = new CommandMessage();
                message =  await client.Receive();
                if(message.Data == "true")
                {
                    this.Hide();
                    chatForm = new Chat(currentUser);
                    chatForm.Show();
                    this.Close();
                }
                else
                    MessageBox.Show($"{message.Data}");
            }
            else
            {
                MessageBox.Show("Please fill in all required fields.");
            }
        }
    }
}