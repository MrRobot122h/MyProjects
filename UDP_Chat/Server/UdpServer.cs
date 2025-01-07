using Newtonsoft.Json;
using Server.Models;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    public class UdpServer
    {
        private readonly UdpClient _udpClient;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private Message _message;
        private ComandService _comandService;


        public UdpServer(int port)
        {
            _udpClient = new UdpClient(port);
            _cancellationTokenSource = new CancellationTokenSource();
            _message = new Message();
            _comandService = new ComandService();
        }

        public void Start()
        {
            Task.Run(() => ActionAsync(_cancellationTokenSource.Token));
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _udpClient.Close();
        }

        private async Task ActionAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = await _udpClient.ReceiveAsync();
                    var remoteEP = result.RemoteEndPoint;
                    string message = Encoding.UTF8.GetString(result.Buffer);
                    _message =  JsonConvert.DeserializeObject<Message>(message);
                    Console.WriteLine($"Received: {_message}");
                    _message.SenderIP = remoteEP.Port.ToString();
                    _message = await _comandService.HandleComand(_message, remoteEP);
                    Console.WriteLine($"Processed: {_message}");
                     

                    if (_message != null)
                    {
                        IPEndPoint receiverIP = remoteEP;

                        if (_message.Comand == "Message")
                        {
                            receiverIP = new IPEndPoint(remoteEP.Address, int.Parse(_message.ReciverIP));
                        }

                        string Data = JsonConvert.SerializeObject(_message);
                        byte[] responseData = Encoding.UTF8.GetBytes(Data);
                        await _udpClient.SendAsync(responseData, responseData.Length, receiverIP);
                    }

                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket error: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }
}
