using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
public class MyTcpClient {

    public static void Main(){
        Connect("127.0.0.1", 25250);
    }

    public static void InitMessage(){
        
    }

    //göra två threads en för att skicka och en för att ta emot
    public static void Connect(String server, int port) {

        try {
            TcpClient client = new TcpClient(server, port);

            NetworkStream stream = client.GetStream();


            string inputString;
            while(true) {
                Console.Write("Enter message: ");
                inputString = Console.ReadLine();

                if (string.IsNullOrEmpty(inputString)) {
                    break;
                }

                Console.Write("Enter package type: ");
                ushort packageType = ushort.Parse(Console.ReadLine()); 

                if (!client.Client.Connected) {
                    break;
                }
                
                byte[] buffer = System.Text.Encoding.ASCII.GetBytes(inputString);
                ushort packetLength = (ushort)buffer.Length;

                List<byte> list = new List<byte>();
                list.AddRange(BitConverter.GetBytes(packageType));
                list.AddRange(BitConverter.GetBytes(packetLength));
                list.AddRange(buffer);


                stream.Write(list.ToArray(), 0, list.Count);

                Console.WriteLine("Sent: {0} \n", string.Join(", ", list));

                byte[] data = new byte[256];

                String responsData = String.Empty;

                Int32 bytes = stream.Read(data, 0, data.Length);
                responsData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Recived: {0}", responsData);
            }

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
    }
}