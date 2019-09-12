using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Monopoly.Lib;
using Monopoly.Board;
using System.Drawing;

/*
 * https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-client-socket-example
 * 
 */
namespace Monopoly.Server
{
    public class Server
    {
        private GameBoard board;
        private Die die;
        private List<Player> players;
        private int currentPlayer;
        private int playerCounter;
        private bool isServerFull;
        private byte[] buffer = new byte[25600 * 4];
        private List<Socket> clientSockets = new List<Socket>();
        private Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //Sets the console windwos and creates a instance of the server object.
        //Finally the server setups the game.
        public static void Main(string[] arg)
        {
            Console.Title = "Server";
            Server gameServer = new Server();
            gameServer.SetupServer();
            Console.ReadLine();
        }

        //Creates board and list of players.
        //Set up server for to recieve players.
        //Create socket for clients to connect too.
        public void SetupServer()
        {
            Console.WriteLine("Setting up server");
            FormatManager.SetupFormatManager(SizeEnum.Large);
            board = new GameBoard(0, 0, Color.Blue, new Size(2048, 2048));

            players = new List<Player>();
            die = new Die();
            isServerFull = false;
            playerCounter = 0;

            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 6969));
            serverSocket.Listen(4);
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
            Console.WriteLine("Server Ready");
        }

        //Displays list of ip addresses connected to server in server consoles.
        private void DisplayConnectedClient()
        {
            foreach (Socket socket in clientSockets)
            {
                IPEndPoint remoteIpEndPoint = socket.RemoteEndPoint as IPEndPoint;
                Console.WriteLine(remoteIpEndPoint.Address);
            }
        }

        //Removes disconnected player from game. If they lost, remove there assets. If they
        //won, just disconnect them.
        private void RemovePlayerFromGame(int disconnectedPlayerId, Socket socket, bool hasLost)
        {
            Player disconnectedPlayer = players[disconnectedPlayerId];

            if (hasLost)
            {
                disconnectedPlayer.BankruptPlayer();
                disconnectedPlayer.LoseAssets();
            }

            if (currentPlayer == disconnectedPlayerId || !hasLost)
            {
                currentPlayer++;

                if (currentPlayer >= players.Count)
                {
                    currentPlayer = 0;
                }

                while (players[this.currentPlayer].IsPlayerBankrupt())
                {
                    currentPlayer++;

                    if (currentPlayer >= players.Count)
                    {
                        currentPlayer = 0;
                    }

                    if (currentPlayer == disconnectedPlayerId)
                        break;
                }

            }

            ServerMessage message = new ServerMessage(MessageEnum.ClearBoard.ToString(), "You were removed.");
            byte[] data = Encoding.ASCII.GetBytes(message.ToString());
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            TellClientsToRefresh();
        }

        //Method that clears the board spaces in prep for receiving a new set of spaces.
        private void ClearBoard(Socket socket)
        {
            board.GetBoardSpaces().Clear();

            ServerMessage message = new ServerMessage(MessageEnum.ClearBoard.ToString(), "Board was clear brother");
            byte[] data = Encoding.ASCII.GetBytes(message.ToString());
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }

        //Method that determines what type of space to retrun via the message header
        private void DetermineGetSpaceType(Socket socket, string messageHeader, string messageBody)
        {
            if (messageBody.Contains("PropertySpace"))
                SendSpacesToClient<PropertySpace>(socket);
            else if (messageBody.Contains("GoToJailSpace"))
                SendSpacesToClient<GoToJailSpace>(socket);
            else if (messageBody.Contains("JailSpace"))
                SendSpacesToClient<JailSpace>(socket);
            else if (messageBody.Contains("CommunityChestSpace"))
                SendSpacesToClient<CommunityChestSpace>(socket);
            else if (messageBody.Contains("ChanceSpace"))
                SendSpacesToClient<ChanceSpace>(socket);
            else if (messageBody.Contains("FreeParkingSpace"))
                SendSpacesToClient<FreeParkingSpace>(socket);
            else if (messageBody.Contains("GoSpace"))
                SendSpacesToClient<GoSpace>(socket);
            else if (messageBody.Contains("RailroadSpace"))
                SendSpacesToClient<RailroadSpace>(socket);
            else if (messageBody.Contains("UtilitySpace"))
                SendSpacesToClient<UtilitySpace>(socket);
            else
                Console.WriteLine("Failed to identify get space");
        }

        //Method that determines what type of space to create via the message header
        private void DetermineSetSpaceType(Socket socket, string messageHeader, string messageBody)
        {
            if (messageHeader == MessageEnum.ClearBoard.ToString())
                SendClientIdToClient(socket);
            else if (messageHeader.Contains("PropertySpace"))
                board.GetBoardSpaces().AddRange(CreateSpaces<PropertySpace>(messageBody, socket));
            else if (messageHeader.Contains("GoToJailSpace"))
                board.GetBoardSpaces().AddRange(CreateSpaces<GoToJailSpace>(messageBody, socket));
            else if (messageHeader.Contains("JailSpace"))
                board.GetBoardSpaces().AddRange(CreateSpaces<JailSpace>(messageBody, socket));
            else if (messageHeader.Contains("CommunityChestSpace"))
                board.GetBoardSpaces().AddRange(CreateSpaces<CommunityChestSpace>(messageBody, socket));
            else if (messageHeader.Contains("ChanceSpace"))
                board.GetBoardSpaces().AddRange(CreateSpaces<ChanceSpace>(messageBody, socket));
            else if (messageHeader.Contains("FreeParkingSpace"))
                board.GetBoardSpaces().AddRange(CreateSpaces<FreeParkingSpace>(messageBody, socket));
            else if (messageHeader.Contains("GoSpace"))
                board.GetBoardSpaces().AddRange(CreateSpaces<GoSpace>(messageBody, socket));

            else if (messageHeader.Contains("RailroadSpace"))
                board.GetBoardSpaces().AddRange(CreateSpaces<RailroadSpace>(messageBody, socket));
            else if (messageHeader.Contains("UtilitySpace"))
                board.GetBoardSpaces().AddRange(CreateSpaces<UtilitySpace>(messageBody, socket));
            else
                Console.WriteLine("Failed to identify set space");
        }

        //Sends message to all clients to pull game data from the server.
        private void TellClientsToRefresh()
        {
            ServerMessage message = new ServerMessage("Refresh", "Used to redraw board");

            foreach (Socket socket in clientSockets)
            {
                byte[] data = Encoding.ASCII.GetBytes(message.ToString());
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            }
        }

        //Advance current player by one on the server
        private void NextPlayerTurn(Socket socket)
        {
            currentPlayer++;
            if (this.currentPlayer >= players.Count)
            {
                this.currentPlayer = 0;
            }

            ServerMessage message = new ServerMessage("NextPlayer", currentPlayer.ToString());
            byte[] data = Encoding.ASCII.GetBytes(message.ToString());
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }

        //Sends current player to the client (whose turn it is).
        private void SendCurrentPlayerToClient(Socket socket)
        {
            ServerMessage message = new ServerMessage("CurrentPlayer", currentPlayer.ToString());

            byte[] data = Encoding.ASCII.GetBytes(message.ToString());
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }

        //Sends a client id to the client. Used for tracking player in the game.
        private void SendClientIdToClient(Socket socket)
        {
            if (isServerFull == false)
            {
                ServerMessage message = new ServerMessage("SetClientId", playerCounter.ToString());
                byte[] data = Encoding.ASCII.GetBytes(message.ToString());
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);

                playerCounter++;

                if (playerCounter >= 4)
                    isServerFull = true;
            }
            else
            {
                ServerMessage message = new ServerMessage("SetClientId", "Server is full");
                byte[] data = Encoding.ASCII.GetBytes(message.ToString());
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            }
        }

        //Method that adds player to server's list of players
        private void AddPlayerToServer(Player player, Socket socket)
        {
            players.Add(player);

            ServerMessage message = new ServerMessage("PlayerCreated", "Player was created");
            byte[] data = Encoding.ASCII.GetBytes(message.ToString());
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }

        //Generic method that creates type of space and returns list of create spaces.
        private List<T> CreateSpaces<T>(String spaceJson, Socket socket)
        {
            List<T> spaces = new List<T>();
            spaces = JsonConvert.DeserializeObject<List<T>>(spaceJson);

            ServerMessage message = new ServerMessage(spaces.GetType().ToString(), "Space was created");

            //board.GetBoardSpaces().AddRange(spaces);

            byte[] data = Encoding.ASCII.GetBytes(message.ToString());
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);

            return spaces;
        }

        //Generic method that sends type of space to client.
        private void SendSpacesToClient<T>(Socket socket)
        {
            List<T> spaces = new List<T>();
            spaces = board.GetBoardSpaces().OfType<T>().ToList<T>();
            ServerMessage message = new ServerMessage(spaces.GetType().ToString(), JsonConvert.SerializeObject(spaces));

            byte[] data = Encoding.ASCII.GetBytes(message.ToString());
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }

        //Sets list of players on the server via the list of players sent from the client.
        private void SetPlayers(Socket socket, List<Player> players)
        {
            ServerMessage message = new ServerMessage("SetPlayers", "Players were set on server");

            this.players = players;

            byte[] data = Encoding.ASCII.GetBytes(message.ToString());
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }

        //Sends list of players to client
        private void SendPlayersToClient(Socket socket)
        {
            ServerMessage message = new ServerMessage("Players", JsonConvert.SerializeObject(players));

            byte[] data = Encoding.ASCII.GetBytes(message.ToString());
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }

        private void AcceptCallback(IAsyncResult asyncResult)
        {
            Socket socket = serverSocket.EndAccept(asyncResult);
            clientSockets.Add(socket);
            Console.WriteLine("Client Connected");
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            Socket socket = (Socket)asyncResult.AsyncState;

            try
            {
                int received = socket.EndReceive(asyncResult);
                byte[] dataBuffer = new byte[received];
                Array.Copy(buffer, dataBuffer, received);

                string text = Encoding.ASCII.GetString(dataBuffer);

                ServerMessage message = new ServerMessage(text);
                string messageHeader = message.GetMessageHeader();
                string messageBody = message.GetMessageBody();

                if (messageHeader == "GetPlayers")
                    SendPlayersToClient(socket);
                else if (messageHeader == "CreatePlayer")
                    AddPlayerToServer(JsonConvert.DeserializeObject<Player>(messageBody), socket);
                else if (messageHeader == "SetPlayers")
                    SetPlayers(socket, JsonConvert.DeserializeObject<List<Player>>(messageBody));
                else if (messageHeader == "GetClientId")
                    SendClientIdToClient(socket);
                else if (messageHeader == "CurrentPlayer")
                    SendCurrentPlayerToClient(socket);
                else if (messageHeader == "NextPlayer")
                    NextPlayerTurn(socket);
                else if (messageHeader == "Get")
                    DetermineGetSpaceType(socket, messageHeader, messageBody);
                else if (messageHeader.Contains("Generic"))
                    DetermineSetSpaceType(socket, messageHeader, messageBody);
                else if (messageHeader == "ClearBoard")
                    ClearBoard(socket);
                else if (messageHeader == "RefreshClients")
                    TellClientsToRefresh();
                else if (messageHeader == "PlayerDisconnect")
                    RemovePlayerFromGame(JsonConvert.DeserializeObject<int>(messageBody), socket, true);
                else if (messageHeader == "PlayerWins")
                    RemovePlayerFromGame(JsonConvert.DeserializeObject<int>(messageBody), socket, false);
                else
                    Console.WriteLine("Failed to identify message from client");


            }
            catch (SocketException ex)
            {
                Console.WriteLine("Client disconnected " + ex.Message);
                clientSockets.Remove(socket);
                DisplayConnectedClient();
            }
        }

        private static void SendCallback(IAsyncResult asyncResult)
        {
            Socket socket = (Socket)asyncResult.AsyncState;
            socket.EndSend(asyncResult);
        }
    }
}
