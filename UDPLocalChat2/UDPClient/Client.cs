using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using UDPServer;

namespace UDPClient
{
    public class Client
    {
        static UdpClient udpClient;
        static IPEndPoint serverEndPoint;
        CommandMessage commandMessage;
        private readonly CancellationTokenSource _cancellationTokenSource;
        ComandsHandler comandsHandler;
        private static Client instance;
        public Client(int port)
        {
            serverEndPoint = new IPEndPoint(IPAddress.Loopback, port);
            _cancellationTokenSource = new CancellationTokenSource();
            udpClient = new UdpClient();
            commandMessage = new CommandMessage();
            comandsHandler = new ComandsHandler();
        }
        public static Client GetInstance(int port = 9000)
        {
            if (instance == null)
            {
                instance = new Client(port);
            }
            return instance;
        }
        
        public async Task<CommandMessage> Receive()
        { 
            var serverResponse = await udpClient.ReceiveAsync();
            string Data = Encoding.UTF8.GetString(serverResponse.Buffer);
            commandMessage = JsonConvert.DeserializeObject<CommandMessage>(Data);
            if (commandMessage != null)
            {
                return comandsHandler.HandleComand(commandMessage);
            }
            return null;
        }
        public static async Task SendMessage(CommandMessage message)
        {
            if (message != null)
            {
                try
                {
                    string Data = JsonConvert.SerializeObject(message);
                    byte[] DataBytes = Encoding.UTF8.GetBytes(Data);
                    await udpClient.SendAsync(DataBytes, DataBytes.Length, serverEndPoint);
                }
                catch (Exception e)
                {
                    MessageBox.Show($"{e.Message}");
                }
            }
        }

    }
}
