using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using System;
using System.Net.Sockets;
using System.Net;

namespace ServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener listen = new TcpListener(IPAddress.Any, 1200);
            Console.WriteLine("Listenning...");
            listen.Start();
            TcpClient client = listen.AcceptTcpClient();
            Console.WriteLine("Client connected");
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[client.ReceiveBufferSize];
            int data = stream.Read(buffer, 0, client.ReceiveBufferSize);
            string ch = Encoding.Unicode.GetString(buffer, 0, data);
            Console.WriteLine("Message recivied: " + ch);
            client.Close();
            Console.ReadKey();
        }
    }
}