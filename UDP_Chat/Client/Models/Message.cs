using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class Message
    {
        public Message() { }
        public Message(string comand, string data, string senderPort, string reciverPort)
        {
            Comand = comand;
            Data = data;
            SenderIP = senderPort;
            ReciverIP = reciverPort;
        }
        public string Comand { get; set; }
        public string Data { get; set; }
        public string SenderIP { get; set; }
        public string ReciverIP { get; set; }

        public override string ToString()
        {
            return $"Comand: {Comand}, Data: {Data}, SenderPort: {SenderIP}, ReciverPort: {ReciverIP}";
        }
    }
}
