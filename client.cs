using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class MyTcpClient {

    public static void Main(){
        Connect();
    }

    public static void InitMessage(){
        
    }

    //göra två threads en för att skicka och en för att ta emot
    public static void Connect() {

        try {
            Int32 port = 25250;
            TcpClient client = new TcpClient(server, port);

            NetworkStream stream = client.GetStream();

            string input_string;
            while(!string.IsNullOrEmpty((input_string = Console.ReadLine()))){

                byte[] buffer = System.Text.Encoding.ASCII.GetBytes(input_string);
                stream.Write(buffer, 0, buffer.Length);

                Console.WriteLine("Sent: {0} \n", System.Text.Encoding.ASCII.GetString(buffer));

            }


            //data = new byte[256];

            //String responsData = String.Empty;

            //Int32 bytes = stream.Read(data, 0, data.Length);
            //responsData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            //Console.WriteLine("Recived: {0}", responsData);

            client.Client.Shutdown(SocketShutdown.Send);
            stream.Close();
            client.Close();

        } catch (ArgumentNullException e) {
            Console.WriteLine("ArgumentNullException: {0}", e);
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }

        Console.WriteLine("\n Press Enter to continue...");
        Console.Read();

    }

}