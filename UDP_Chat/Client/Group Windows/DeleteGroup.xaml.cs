using Client.Models;
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

namespace Client.Group_Windows
{
    /// <summary>
    /// Interaction logic for DeleteGroup.xaml
    /// </summary>
    public partial class DeleteGroup : Window
    {
        Message message;
        User user;
        public DeleteGroup(User user)
        {
            InitializeComponent();
            this.user = user;
        }

        private async void Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Name_TextBox.Text != "")
            {
                message = new Message("Delete group", Name_TextBox.Text, null, null);
                await ClientCode.SendMessage(message);
            }
            else
                MessageBox.Show("Enter the name!");

        }
    }
}
