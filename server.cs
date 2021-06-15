using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

using GameServer;

public class MyTcpListener
{

    static object _lock = new object();
    static Dictionary<int, PlayerConnection> client_list = new Dictionary<int, PlayerConnection>();
    public static void Main()
    {
        TcpListener server = null;

        Int32 port = 25250;
        IPAddress localAddr = IPAddress.Parse("127.0.0.1");

        server = new TcpListener(localAddr, port);
        server.Start();

        int count = 1;
        Console.WriteLine("Waiting for a connection... ");
        while (true)
        {

            TcpClient connection = server.AcceptTcpClient();

            PlayerConnection player = new PlayerConnection(connection);
            lock (_lock) { client_list.Add(count, player); }

            Thread thread = new Thread(client_thread);
            thread.Start(count);
            count++;
        }

    }
    public static void client_thread(object o)
    {

        PlayerConnection player;
        lock (_lock) player = client_list[(int)o];
        Console.WriteLine("Conection astablished from: {0}", player.client.Client.RemoteEndPoint);


		//client appendar en short i början av packetet och den här grejen läser av 2 bytes vilket är shorten, t.ex 3 0.
        // Layout: (short) package type, (short) package length, (byte[]) package content.
        Byte[] byteLength = new Byte[2];
        Byte[] packageType = new Byte[2]; //gör om skit namn
        while(true) {

            Byte[] bytes = new Byte[1024];
            String data = "";

            NetworkStream stream = player.client.GetStream();

            int countRead = stream.Read(packageType, 0, packageType.Length);
            ushort package_type = BitConverter.ToUInt16(packageType);

            countRead = stream.Read(byteLength, 0, byteLength.Length);
            if (countRead < byteLength.Length)
            {
                throw new InvalidOperationException("packet to short");
            }

            ushort bytesToRead = BitConverter.ToUInt16(byteLength);

            Console.WriteLine("package from: {0}", player.client.Client.RemoteEndPoint);
            Console.WriteLine("package type: {0}", package_type);
            Console.WriteLine("package length: {0}", bytesToRead);

            int i;
            while ((i = stream.Read(bytes, 0, Math.Min(bytesToRead, bytes.Length))) != 0)
            {
                bytesToRead -= (ushort)i;
                data += System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                Console.WriteLine("package data: {0} \n", data);
            }

        }
        Console.WriteLine("Shutting down connection to {0}", player.client.Client.RemoteEndPoint);
        lock (_lock) client_list.Remove((int)o);
        player.client.Client.Shutdown(SocketShutdown.Both);
        player.client.Close();
    }
}
