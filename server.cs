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
        while (true)
        {
            Console.WriteLine("Waiting for a connection... ");

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
        Console.WriteLine("Coneected!");

        Byte[] bytes = new Byte[1024];
        String data = "";

        NetworkStream stream = player.client.GetStream();

		//client appendar en short i början av packetet och den här grejen läser av 2 bytes vilket är shorten
        Byte[] byteLength = new Byte[2];
        int countRead = stream.Read(byteLength, 0, byteLength.Length);
        if (countRead < byteLength.Length)
        {
            throw new InvalidOperationException("packet to short");
        }

        ushort bytesToRead = BitConverter.ToUInt16(byteLength);

        int i;
        while ((i = stream.Read(bytes, 0, Math.Min(bytesToRead, bytes.Length))) != 0)
        {
            bytesToRead -= (ushort)i;
            data += System.Text.Encoding.ASCII.GetString(bytes, 0, i);
			Console.WriteLine("asd");
        }
        Console.WriteLine("received: {0}", data);
        data = data.ToUpper();

        byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

        //stream.Write(msg, 0, msg.Length);
        Console.WriteLine("Sent: {0}", data);

        //on read someting
        //player.packet_queue.add)(("blalbblala))

        lock (_lock) client_list.Remove((int)o);
        player.client.Client.Shutdown(SocketShutdown.Both);
        player.client.Close();

    }
}
