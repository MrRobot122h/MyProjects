using Client.Models;
using Newtonsoft.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ClientCode client;
        DatePicker? datePicker;
        Message message = new Message();
        User currentUser;
        UI UIform;

        public MainWindow()
        {
            InitializeComponent();
            client = ClientCode.GetInstance();
        }

        private void SignUpRB_Checked(object sender, RoutedEventArgs e)
        {
            if (SignUpRB.IsChecked == true)
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

            if (message.Comand == "Login" && birthDate == null)
            {
                birthDate = DateTime.Now.AddYears(-22).ToShortDateString();
            }

            currentUser = new User(LoginTB.Text, PasswordTB.Text, birthDate, 1, null);

            if (string.IsNullOrEmpty(LoginTB.Text) || string.IsNullOrEmpty(PasswordTB.Text))
            {
                MessageBox.Show("Please fill in all required fields.");
                return;
            }

            if (message.Comand == "SignUp")
            {
                if (birthDate == null)
                {
                    MessageBox.Show("Please fill birth date.");
                    return;
                }

                await SignUp(currentUser);
            }
            else if (message.Comand == "Login")
            {
                await Login(currentUser);
            }

        }

        private async Task SignUp(User user)
        {
            message.Data = JsonConvert.SerializeObject(user);
            await ClientCode.SendMessage(message);
            message = await client.Receive();
            if (message.Data == "true")
            {
                this.Hide();
                UIform = new UI(user);
                UIform.Show();
                this.Close();
            }
            else
                MessageBox.Show($"SignUp failed: {message.Data}");
        }

        private async Task Login(User user)
        {
            message.Data = JsonConvert.SerializeObject(user);
            await ClientCode.SendMessage(message);
            message = await client.Receive();
            if (message.Data == "true")
            {
                this.Hide();
                UIform = new UI(user);
                UIform.Show();
                this.Close();
            }
            else
                MessageBox.Show($"Login failed: {message.Data}");
        }
    }
}