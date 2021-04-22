using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GameServer {
    public class PlayerConnection {

        public Queue<String> packetQueue = new Queue<String>();
        public TcpClient client;

        public readonly object packetQueueLock = new object();
        public PlayerConnection(TcpClient client){
        this.client = client; 
        }
    }

}