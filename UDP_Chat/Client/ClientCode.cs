using Client.Models;
using Client.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Client
{
    public class ClientCode
    {
        static UdpClient udpClient;
        static IPEndPoint serverEndPoint;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private static ClientCode instance;
        private Message _message;
        private ComandService _comandService;


        private ClientCode(int port)
        {
            serverEndPoint = new IPEndPoint(IPAddress.Loopback, port);
            _cancellationTokenSource = new CancellationTokenSource();
            _message = new Message();
            udpClient = new UdpClient();
            _comandService = new ComandService();
        }

        public static ClientCode GetInstance(int port = 9000)
        {
            if (instance == null)
            {
                instance = new ClientCode(port);
            }
            return instance;
        }

        public void ChangeComandService(UI obj)
        {
            _comandService = new ComandService(obj);
        }
        public async Task<Message> JustReceive()
        {
            try
            {
                var serverResponse = await udpClient.ReceiveAsync();
                string data = Encoding.UTF8.GetString(serverResponse.Buffer);

                if (!string.IsNullOrEmpty(data))
                {
                    var commandMessage = JsonConvert.DeserializeObject<Message>(data);

                    if (commandMessage != null)
                    {
                        return commandMessage;
                    }
                    else
                    {
                        Console.WriteLine("Отримане повідомлення не відповідає формату Message.");
                    }
                }
                else
                {
                    Console.WriteLine("Отримано порожнє повідомлення.");
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Помилка сокета: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Інша помилка: {ex.Message}");
            }

            return null;
        }


        public async Task<Message> Receive()
        {
            try
            {
                var serverResponse = await udpClient.ReceiveAsync();
                string data = Encoding.UTF8.GetString(serverResponse.Buffer);

                if (!string.IsNullOrEmpty(data))
                {
                    var commandMessage = JsonConvert.DeserializeObject<Message>(data);

                    if (commandMessage != null)
                    {
                        return await _comandService.HandleComand(commandMessage);
                    }
                    else
                    {
                        Console.WriteLine("Отримане повідомлення не відповідає формату Message.");
                    }
                }
                else
                {
                    Console.WriteLine("Отримано порожнє повідомлення.");
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Помилка сокета: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Інша помилка: {ex.Message}");
            }

            return null;
        }

        public static async Task SendMessage(Message message)
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
