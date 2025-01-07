using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    [Serializable]
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string BirthDate { get; set; } 
        public int Online { get; set; }
        public string? IP { get; set; }
    }
}
