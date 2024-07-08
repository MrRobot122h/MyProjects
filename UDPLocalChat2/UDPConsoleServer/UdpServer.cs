using Newtonsoft.Json;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UdpServer
{
    private readonly UdpClient _udpClient;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private CommandMessage _message;
    private ComandsHandler comandsHandler;

    public UdpServer(int port)
    {
        _udpClient = new UdpClient(port);
        _cancellationTokenSource = new CancellationTokenSource();
        _message = new CommandMessage();
        comandsHandler = new ComandsHandler();
        DataBaseHelper.CreateDatabase();
        DataBaseHelper.CreateTable();
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

                _message = JsonConvert.DeserializeObject<CommandMessage>(message);

                Console.WriteLine($"Received: {_message}");
                _message = comandsHandler.HandleComand(_message, remoteEP);
                Console.WriteLine($"Processed: {_message}");

                if (_message != null)
                {
                    IPEndPoint receiverIP = remoteEP;

                    if (_message.Comand == "Message")
                    {
                        receiverIP = new IPEndPoint(remoteEP.Address, int.Parse(_message.Reciver));
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
        finally
        {
            _udpClient.Close();
        }
    }
}



    
