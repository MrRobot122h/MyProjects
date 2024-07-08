namespace UDPClient
{
    [Serializable]
    public class CommandMessage
    {
        public CommandMessage() { }
        public CommandMessage(string comand, string data, string senderPort, string reciverPort)
        {
            Comand = comand;
            Data = data;
            Sender = senderPort;
            Reciver = reciverPort;
        }
        public string Comand { get; set; }
        public string Data { get; set; }
        public string Sender { get; set; }
        public string Reciver { get; set; }
        public override string ToString() {
            return $"Comand: {Comand}, Data: {Data}, Sender: {Sender}, Reciver: {Reciver}";
        }
    }
}


