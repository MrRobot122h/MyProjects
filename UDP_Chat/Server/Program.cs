using Server;

public class Program
{
    public static void Main(string[] args)
    {
        UdpServer udpServer = new UdpServer(9000);
        udpServer.Start();

        Console.WriteLine("Press any key to stop the server...");
        Console.ReadKey();
        udpServer.Stop();
    }
}

