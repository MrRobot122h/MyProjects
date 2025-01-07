using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    [Serializable]
    public class User
    {
        public User(string name, string password, 
            string? birthdate, int online, string? ip)
        {
            Login = name;
            Password = password;
            BirthDate = birthdate;
            Online = online;
            IP = ip;
        }
        public string Login { get; set; }
        public string Password { get; set; }
        public string BirthDate { get; set; }
        public int Online { get; set; }
        public string? IP { get; set; }
    }
}
