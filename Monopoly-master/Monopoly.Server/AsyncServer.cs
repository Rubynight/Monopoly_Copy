using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly.Server
{
    public class AsyncServer
    {
        private static byte[] buffer = new byte[4096];
        private static List<Socket> clientSockets = new List<Socket>();
        private static GameInstance gameInstance;
        private static Socket serverSocket =
            new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public static void Main(string[] arg)
        {
            Console.Title = "Server";
            SetupServer();
            Console.ReadLine();
        }

        public static void SetupServer()
        {
            Console.WriteLine("Setting up server");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 6969));
            serverSocket.Listen(4);
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            Console.WriteLine("Server Started");
        }

        private static void AcceptCallback(IAsyncResult asyncResult)
        {
            Socket socket = serverSocket.EndAccept(asyncResult);
            clientSockets.Add(socket);
            Console.WriteLine("Client Connected");
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);

            foreach (Socket s in clientSockets)
                Console.WriteLine(s.ToString());
        }

        private static void ReceiveCallback(IAsyncResult asyncResult)
        {
            Socket socket = (Socket)asyncResult.AsyncState;

            try
            {
                int received = socket.EndReceive(asyncResult);
                byte[] dataBuffer = new byte[received];
                Array.Copy(buffer, dataBuffer, received);

                string text = Encoding.ASCII.GetString(dataBuffer);
                Console.WriteLine("Text received: " + text);

                string response = string.Empty;

                if (text.ToLower() == "get time")
                {
                    response = DateTime.Now.ToLongTimeString();
                }
                else if(text.ToLower() == "game data")
                {
                    response = "Loading game data";
                }
                else
                {
                    response = "Invalid Request";
                }

                byte[] data = Encoding.ASCII.GetBytes(response);
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            }
            catch (SocketException ex)
            {
                Console.WriteLine("Client disconnected");
                clientSockets.Remove(socket);
            }
        }


        private static void SendCallback(IAsyncResult asyncResult)
        {
            Socket socket = (Socket)asyncResult.AsyncState;
            socket.EndSend(asyncResult);
        }
    }
}
