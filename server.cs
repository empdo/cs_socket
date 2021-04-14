using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class MyTcpListener {

	static object _lock = new object();
	static Dictionary<int, TcpClient> client_list = new Dictionary<int, TcpClient>();

	public static void Main(){
		TcpListener server = null;

		Int32 port = 25250;
		IPAddress localAddr = IPAddress.Parse("127.0.0.1");

		server = new TcpListener(localAddr, port);
		server.Start();

		int count = 1;
		while(true){
			Console.WriteLine("Waiting for a connection... ");

			TcpClient connection = server.AcceptTcpClient();
			lock (_lock) client_list.Add(count, connection);

			Thread thread = new Thread(client_thread);
			Queue qt = new Queue();
			thread.Start(count);
			count++;
		}

		Console.WriteLine("\nHit enter to continue...");
		Console.Read();

	}

	public static void client_thread(object o) {

		TcpClient connection;
		lock (_lock) connection = client_list[(int)o];	
		Console.WriteLine("Coneected!");

		Byte[] bytes = new Byte[256];
		String data = null;

		NetworkStream stream = connection.GetStream();

		int i;

		while((i = stream.Read(bytes, 0, bytes.Length)) != 0){
			data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
			Console.WriteLine("received: {0}", data);

			data = data.ToUpper();

			byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

			//stream.Write(msg, 0, msg.Length);
			Console.WriteLine("Sent: {0}", data);
		}	

		lock (_lock) client_list.Remove((int)o);
		connection.Client.Shutdown(SocketShutdown.Both);
		connection.Close();

	}
}
