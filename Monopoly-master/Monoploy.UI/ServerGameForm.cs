using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Monopoly.Lib;
using Monopoly.Board;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Diagnostics;
/*
* Game form used for when player is playing a server game. 
* Objects are sent over the server via JSON using newtonsoft json.net
* https://www.newtonsoft.com/json
* 
* In order to use newtonsoft json.net, we need to set the properties we 
* want to send over the network to the C# style of declaring properties.
* So we need to use public varName {get;set;}, instead of getter and setter
* methods.
*/

namespace Monoploy.UI
{
    public partial class ServerGameForm : Form
    {
        //Server Side
        private List<Player> players;
        private GameBoard board;
        private int currentPlayer;
        private int clientPlayerId;
        private string ipAddress;
        private Color playerColor;

        //Client Side 
        private Camera camera;
        private GameTimer timer;
        private Die die;
        private GameObject clickedObject;
        private Socket clientSocket;
        private int windowWidth;
        private int windowHeight;
        private Form prevFrom;
        private int serverId;
        private Process process;

        //Constructor that holds the a instance of the main menu to send player back to the main menu.
        //Constructor calls all method to get a working game up. This includes creating the game objects, the window dimensions,
        //connecting to the server, and getting a client id.
        public ServerGameForm(Form prevFrom)
        {
            this.prevFrom = prevFrom;
            SetupWindow();
            SetupObjects();
            LoopConnect();

            if (clientSocket.Connected)
            {
                GetClientIdFromServer();

                if (clientPlayerId != -1)
                {
                    SetupGame(SizeEnum.Large);
                    SendRefreshMessageToServer();
                    Show();
                }
            }
        }

        //Creates a instance of a server.
        //Allows player to host a game on their computer.
        public void StartServer()
        {
            //Checks if there already a server instance running on client's cpmputer.
            if (Process.GetProcessesByName("Monopoly.Server").Length > 0)
            {
                MessageBox.Show("Server is already running.");
                return;
            }

            process = Process.Start(@"..\..\..\Monopoly.Server\bin\Debug\Monopoly.Server.exe");
            serverId = process.Id;
        }

        //Attempts to connect to a server based off of user inputed ip address.
        //After 10 failed attempts, the user is sent back to the main menu.
        private void LoopConnect()
        {
            int timeout = 0;

            while (!clientSocket.Connected && timeout < 10)
            {
                using (InputServerIP ipForm = new InputServerIP(this))
                {
                    Hide();
                    ipForm.ShowDialog();
                }

                try
                {
                    IPAddress serverAddress;
                    IPAddress.TryParse(ipAddress, out serverAddress);

                    if (serverAddress != null)
                        clientSocket.Connect(serverAddress, 6969);

                }
                catch (SocketException ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                timeout++;
            }

            if (timeout >= 10)
            {
                MessageBox.Show("Failed to connect to the server.");
                prevFrom.Show();
                this.Close();
            }
            else
            {
                Show();
                Console.WriteLine("Connected");
            }
        }

        //Creates a player object and sends it to the server to be stored.
        //Will be used in game.
        private void CreatePlayer()
        {
            Player p = new Player(clientPlayerId, board.GetStartingSpace(), playerColor);
            ServerMessage messageToServer = new ServerMessage("CreatePlayer", JsonConvert.SerializeObject(p));

            byte[] buffer = Encoding.ASCII.GetBytes(messageToServer.ToString());
            clientSocket.Send(buffer);

            byte[] receivedBuffer = new byte[25600 * 2];
            int rec = clientSocket.Receive(receivedBuffer);

            byte[] data = new byte[rec];
            Array.Copy(receivedBuffer, data, rec);

            ServerMessage messageFromServer = new ServerMessage(Encoding.ASCII.GetString(data));
            Console.WriteLine(messageFromServer.GetMessageBody());
        }

        //Ask server to send list of player back to the client.
        //Method than returns this list.
        private List<Player> GetPlayersFromServer()
        {
            ServerMessage messageToServer = new ServerMessage("GetPlayers", "Retrieve players from server");

            byte[] buffer = Encoding.ASCII.GetBytes(messageToServer.ToString());
            clientSocket.Send(buffer);

            byte[] receivedBuffer = new byte[25600 * 2];
            int rec = clientSocket.Receive(receivedBuffer);

            byte[] data = new byte[rec];
            Array.Copy(receivedBuffer, data, rec);

            ServerMessage messageFromServer = new ServerMessage(Encoding.ASCII.GetString(data));
            Console.WriteLine(messageFromServer.GetMessageBody());

            return JsonConvert.DeserializeObject<List<Player>>(messageFromServer.GetMessageBody());
        }

        //Collection of methods used to retrieve game data from server.
        //Pulls the board space data, player data, and the who is the current player (player turn).
        private void GetDataFromServer()
        {
            List<Space> boardSpaces = GetSpacesFromServer();
            players = GetPlayersFromServer();
            GetCurrentPlayer();
        }

        //Collections of method calls that asks the server to send back each type 
        //space back to the user. Add these spaces to a temp list and copy list
        //to board space list.
        //We pull each space seperately since json doesn't handle parent child relationships well.
        private List<Space> GetSpacesFromServer()
        {
            List<Space> boardSpaces = new List<Space>();
            boardSpaces.AddRange(ReceiveSpacesFromServer<PropertySpace>());
            boardSpaces.AddRange(ReceiveSpacesFromServer<CommunityChestSpace>());
            boardSpaces.AddRange(ReceiveSpacesFromServer<ChanceSpace>());
            boardSpaces.AddRange(ReceiveSpacesFromServer<FreeParkingSpace>());
            boardSpaces.AddRange(ReceiveSpacesFromServer<GoSpace>());
            boardSpaces.AddRange(ReceiveSpacesFromServer<GoToJailSpace>());
            boardSpaces.AddRange(ReceiveSpacesFromServer<JailSpace>());
            boardSpaces.AddRange(ReceiveSpacesFromServer<RailroadSpace>());
            boardSpaces.AddRange(ReceiveSpacesFromServer<UtilitySpace>());
            board.SetBoardSpaces(boardSpaces);
            return boardSpaces;
        }

        //Collection of methods that send the clients game data to the server.
        //Tells server to clear board data for perp in recieving spaces.
        //Sends each type of space to the server.
        //Sends player data to the server.
        //Tells server to advance player turn.
        //Tell server to ask other clients to get server data so other clients can
        //get the update game.
        private void SendDataToServer()
        {
            SendClearBoardToServer();
            SendBoardSpacesToServer<PropertySpace>();
            SendBoardSpacesToServer<CommunityChestSpace>();
            SendBoardSpacesToServer<ChanceSpace>();
            SendBoardSpacesToServer<FreeParkingSpace>();
            SendBoardSpacesToServer<GoSpace>();
            SendBoardSpacesToServer<GoToJailSpace>();
            SendBoardSpacesToServer<JailSpace>();
            SendBoardSpacesToServer<RailroadSpace>();
            SendBoardSpacesToServer<UtilitySpace>();
            SendPlayerDataToServer();
            SendAdvancePlayersTurn();
            SendRefreshMessageToServer();
        }

        //Tell server to ask other clients to get server data so other clients can
        //get the update game.
        private void SendRefreshMessageToServer()
        {
            ServerMessage messageToServer = new ServerMessage("RefreshClients", "Tells other clients to get data from the server");

            byte[] buffer = Encoding.ASCII.GetBytes(messageToServer.ToString());
            clientSocket.Send(buffer);

            byte[] receivedBuffer = new byte[25600 * 2];
            int rec = clientSocket.Receive(receivedBuffer);

            byte[] data = new byte[rec];
            Array.Copy(receivedBuffer, data, rec);

            ServerMessage messageFromServer = new ServerMessage(Encoding.ASCII.GetString(data));
            Console.WriteLine(messageFromServer.GetMessageBody());
        }

        //Tells server to advance player turn.
        private void SendAdvancePlayersTurn()
        {
            ServerMessage messageToServer = new ServerMessage("NextPlayer", "It's the next player's turn");

            byte[] buffer = Encoding.ASCII.GetBytes(messageToServer.ToString());
            clientSocket.Send(buffer);

            byte[] receivedBuffer = new byte[25600 * 2];
            int rec = clientSocket.Receive(receivedBuffer);

            byte[] data = new byte[rec];
            Array.Copy(receivedBuffer, data, rec);

            ServerMessage messageFromServer = new ServerMessage(Encoding.ASCII.GetString(data));
            Console.WriteLine(messageFromServer.GetMessageBody());
        }

        //Sends player data to the server.
        private void SendPlayerDataToServer()
        {
            ServerMessage messageToServer = new ServerMessage("SetPlayers", JsonConvert.SerializeObject(players));
            byte[] buffer = Encoding.ASCII.GetBytes(messageToServer.ToString());
            clientSocket.Send(buffer);

            byte[] receivedBuffer = new byte[25600 * 2];
            int rec = clientSocket.Receive(receivedBuffer);

            byte[] data = new byte[rec];
            Array.Copy(receivedBuffer, data, rec);

            ServerMessage messageFromServer = new ServerMessage(Encoding.ASCII.GetString(data));
            Console.WriteLine(messageFromServer.GetMessageBody());
        }

        //Tells server to clear board data for perp in recieving spaces.
        private void SendClearBoardToServer()
        {
            ServerMessage messageToServer = new ServerMessage("ClearBoard", "Clear that board boy");

            byte[] buffer = Encoding.ASCII.GetBytes(messageToServer.ToString());
            clientSocket.Send(buffer);

            byte[] receivedBuffer = new byte[25600 * 2];
            int rec = clientSocket.Receive(receivedBuffer);

            byte[] data = new byte[rec];
            Array.Copy(receivedBuffer, data, rec);

            ServerMessage messageFromServer = new ServerMessage(Encoding.ASCII.GetString(data));
            Console.WriteLine(messageFromServer.GetMessageBody());
        }

        //Ask server to send the current player to client.
        private void GetCurrentPlayer()
        {
            ServerMessage messageToServer = new ServerMessage("CurrentPlayer", "Get that player");
            byte[] buffer = Encoding.ASCII.GetBytes(messageToServer.ToString());
            clientSocket.Send(buffer);

            byte[] receivedBuffer = new byte[25600 * 2];
            int rec = clientSocket.Receive(receivedBuffer);

            byte[] data = new byte[rec];
            Array.Copy(receivedBuffer, data, rec);
            ServerMessage messageFromServer = new ServerMessage(Encoding.ASCII.GetString(data));

            currentPlayer = int.Parse(messageFromServer.GetMessageBody());
            Console.WriteLine("Current Player is " + currentPlayer);
        }

        //Generic method that ask server to send a type of space to the client.
        //The method than returns the list of spaces.
        public List<T> ReceiveSpacesFromServer<T>()
        {
            List<T> spaces = new List<T>();

            ServerMessage messageToServer = new ServerMessage("Get", spaces.GetType().ToString());

            byte[] buffer = Encoding.ASCII.GetBytes(messageToServer.ToString());
            clientSocket.Send(buffer);

            byte[] receivedBuffer = new byte[25600 * 4];
            int rec = clientSocket.Receive(receivedBuffer);

            byte[] data = new byte[rec];
            Array.Copy(receivedBuffer, data, rec);

            ServerMessage messageFromServer = new ServerMessage(Encoding.ASCII.GetString(data));

            spaces = JsonConvert.DeserializeObject<List<T>>(messageFromServer.GetMessageBody());
            return spaces;
        }

        //Generic method that sends a type of space to the server.
        public void SendBoardSpacesToServer<T>()
        {
            List<T> spaces = new List<T>();
            spaces = board.GetBoardSpaces().OfType<T>().ToList<T>();

            ServerMessage messageToServer = new ServerMessage(spaces.GetType().ToString(), JsonConvert.SerializeObject(spaces));

            byte[] buffer = Encoding.ASCII.GetBytes(messageToServer.ToString());
            clientSocket.Send(buffer);

            byte[] receivedBuffer = new byte[25600 * 2];
            int rec = clientSocket.Receive(receivedBuffer);

            byte[] data = new byte[rec];
            Array.Copy(receivedBuffer, data, rec);

            ServerMessage messageFromServer = new ServerMessage(Encoding.ASCII.GetString(data));
            Console.WriteLine(messageFromServer.GetMessageBody());
        }

        //Tells server that you are disconnecting from the server.
        private void TellServerToDisconnectPlayer()
        {
            ServerMessage messageToServer = new ServerMessage("PlayerDisconnect", JsonConvert.SerializeObject(clientPlayerId));

            byte[] buffer = Encoding.ASCII.GetBytes(messageToServer.ToString());
            clientSocket.Send(buffer);

            byte[] receivedBuffer = new byte[25600 * 2];
            int rec = clientSocket.Receive(receivedBuffer);

            byte[] data = new byte[rec];
            Array.Copy(receivedBuffer, data, rec);

            Console.WriteLine("Successfully disconnected");

            MessageBox.Show("closing");
        }

        //Tells server that the player has won and is disconnecting
        private void TellServerPlayerWon()
        {
            ServerMessage messageToServer = new ServerMessage("PlayerWins", JsonConvert.SerializeObject(clientPlayerId));

            byte[] buffer = Encoding.ASCII.GetBytes(messageToServer.ToString());
            clientSocket.Send(buffer);

            byte[] receivedBuffer = new byte[25600 * 2];
            int rec = clientSocket.Receive(receivedBuffer);

            byte[] data = new byte[rec];
            Array.Copy(receivedBuffer, data, rec);

            Console.WriteLine("Successfully disconnected");

            MessageBox.Show("closing");
            Close();
        }

        //Tell server to send a client id to the player so player can be tracked by the server.
        //There can only be a max of four client ids per games.
        private void GetClientIdFromServer()
        {
            ServerMessage messageToServer = new ServerMessage("GetClientId", "Can I get a Client Id?");

            byte[] buffer = Encoding.ASCII.GetBytes(messageToServer.ToString());
            clientSocket.Send(buffer);

            byte[] receivedBuffer = new byte[25600 * 2];
            int rec = clientSocket.Receive(receivedBuffer);

            byte[] data = new byte[rec];
            Array.Copy(receivedBuffer, data, rec);

            ServerMessage messageFromServer = new ServerMessage(Encoding.ASCII.GetString(data));

            if (messageFromServer.GetMessageBody() == "Server is full")
            {
                MessageBox.Show("Game is full.");
                clientPlayerId = -1;
                Close();
            }
            else
            {
                clientPlayerId = int.Parse(messageFromServer.GetMessageBody());
                this.Text = "Client Id - " + clientPlayerId;

            }
        }

        //Create game objects and client socket used for connecting to the server.
        private void SetupObjects()
        {
            //Init objects
            camera = new Camera(windowWidth, windowHeight, 0, 0);
            die = new Die();
            timer = new GameTimer();
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        //Setups the form window
        private void SetupWindow()
        {
            //Init Form, should always be the first command executed. SetStyle is rendering hints to tell the
            //form how to draw. These hints will prevent flickering.
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);

            windowHeight = 900;
            windowWidth = 1600;
            SetBounds(0, 0, windowWidth, windowHeight);
        }

        //Collections of methods related to setting up components of the game.
        //Setups format manager that is used to store how text should be drawn.
        //SetupBoard used to creating the game board and spaces.
        //Setups a timer that checks for refresh messagse from the server.
        private void SetupGame(SizeEnum size)
        {
            FormatManager.SetupFormatManager(size);
            SetupBoard(size);
            timer.SetupGameTimer(UpdateFromServer);
            GetSpacesFromServer();
            CreatePlayer();
            GetCurrentPlayer();
            players = GetPlayersFromServer();
            camera.centerOnGameObject(board.GetStartingSpace());
        }

        //Creates a new board and adds it and its spaces to the gameobjects list.
        private void SetupBoard(SizeEnum size)
        {
            board = new GameBoard(0, 0, Color.Blue, new Size(2048, 2048));
        }

        //Timer that goes off every second to check if there is any refresh messages.
        //If there is, refresh game.
        public void UpdateFromServer(object sender, EventArgs e)
        {
            if (clientSocket.Available != 0)
            {
                byte[] receivedBuffer = new byte[25600 * 2];
                int rec = clientSocket.Receive(receivedBuffer);

                byte[] data = new byte[rec];
                Array.Copy(receivedBuffer, data, rec);

                ServerMessage messageFromServer = new ServerMessage(Encoding.ASCII.GetString(data));

                if (messageFromServer.GetMessageHeader() == "Refresh")
                {
                    GetDataFromServer();
                    camera.centerOnGameObject(players[currentPlayer]);
                    Invoke(new Action(() => this.Refresh()));
                }
            }
        }

        //Form method that runs when refresh is called. Also provies the graphic object that is
        //used to draw on the form
        private void GameForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (board != null)
            {
                foreach (Space space in board.GetBoardSpaces())
                {
                    space.Draw(g, space.AdjustForXOffset(camera.GetXOffset()), space.AdjustForYOffset(camera.GetYOffset()));

                    if (clickedObject is IClickToDrawText clickToDrawText)
                    {
                        g.DrawString(clickToDrawText.TextToDraw(), FormatManager.GetPlayerFont(), new SolidBrush(Color.Black), new PointF(0, windowHeight - FormatManager.GetPlayerFont().Height * 2));
                    }
                }

                foreach (Player player in players)
                {
                    player.Draw(g, player.AdjustForXOffset(camera.GetXOffset()), player.AdjustForYOffset(camera.GetYOffset()), currentPlayer, clientPlayerId);
                }
            }

        }

        //Form method activates when a user clicks on the form. It provides mouse_event_args which gives
        //the x and y location of the mouse. 
        //This method is used to determine if a object was clicked and if so, center on it and call any related methods.
        private void GameForm_MouseClick(object sender, MouseEventArgs e)
        {
            foreach (Space gameObject in board.GetBoardSpaces())
            {
                if (gameObject.WasClicked(e.X, e.Y, gameObject.AdjustForXOffset(camera.GetXOffset()), gameObject.AdjustForYOffset(camera.GetYOffset())))
                {
                    clickedObject = gameObject;
                    gameObject.ClickAction();
                    camera.centerOnGameObject(gameObject);
                    break;
                }
            }

            this.Refresh();
        }

        //Sets the form style to borderless and maximized
        private void GameForm_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            //this.FormBorderStyle = FormBorderStyle.None;
            //this.WindowState = FormWindowState.Maximized;
        }

        //Form method that activates when a key is press. Used to perfrom key relate actions in the game, such
        //as rolling the dice, moving the camera, etc.
        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Escape)
            {
                players[clientPlayerId].BankruptPlayer();
                this.Close();
            }

            if (e.KeyCode == Keys.Up)
                camera.move(0, -Camera.CAMERA_SPEED);

            if (e.KeyCode == Keys.Down)
                camera.move(0, Camera.CAMERA_SPEED);

            if (e.KeyCode == Keys.Left)
                camera.move(-Camera.CAMERA_SPEED, 0);

            if (e.KeyCode == Keys.Right)
                camera.move(Camera.CAMERA_SPEED, 0);



            this.Refresh();


            //Doesn't allow player action until game has four players.
            if (!IsFullGame())
            {
                Console.WriteLine("Game is full");
                return;

            }

            //Form method that activates when a key is press. Used to perfrom key relate actions in the game, such
            //as rolling the dice, moving the camera, etc. Only works if it's the clients turn.
            if (e.KeyCode == Keys.R && currentPlayer == clientPlayerId)
            {
                if (!players[currentPlayer].IsPlayerBankrupt())
                    MoveCurrentPlayer();

                clickedObject = null;
                this.Refresh();

                if (CheckIfPlayerWon())
                {
                    MessageBox.Show("Player " + currentPlayer + " has won");
                    this.Close();
                }

                SendDataToServer();
            }

            //Debug command
            if (e.KeyCode == Keys.U)
            {
                foreach (Space space in board.GetBoardSpaces())
                {
                    if (space is PropertySpace p)
                    {
                        if (p.GetOwner() != null)
                            p.GetProperty().IncreaseNumOfHouses();
                    }
                }
            }

            //Debug command
            if (e.KeyCode == Keys.L)
            {
                players[currentPlayer].RemoveMoney(100);
            }

            this.Refresh();
        }

        //Checks if player list is four in size to determine if game is full.
        private bool IsFullGame()
        {
            return players.Count == 4;
        }

        //Loops through player list and check if all but one player is bankrupt, if there
        //is only one not bankrupt, return true, else false.
        private bool CheckIfPlayerWon()
        {
            int numOfBankrupts = 0;

            foreach (Player p in players)
            {
                if (p.IsPlayerBankrupt())
                    numOfBankrupts++;
            }

            if (numOfBankrupts == players.Count - 1)
                return true;
            else
                return false;
        }

        //Called when player presses R and isn't bankrupt
        private void MoveCurrentPlayer()
        {
            Player currentPlayer = players[this.currentPlayer];

            if (!currentPlayer.IsPlayerJailed())
                MoveCurrentPlayerByDiceRoll();

            camera.centerOnGameObject(currentPlayer);
            this.Refresh();

            PlayerAction(currentPlayer, board);
            this.Refresh();


            this.currentPlayer++;

            if (this.currentPlayer >= players.Count)
            {
                this.currentPlayer = 0;
            }

            while (players[this.currentPlayer].IsPlayerBankrupt())
            {
                this.currentPlayer++;

                if (this.currentPlayer >= players.Count)
                {
                    this.currentPlayer = 0;
                }
            }


            System.Threading.Thread.Sleep(2000);
            camera.centerOnGameObject(players[this.currentPlayer]);
        }
        //Method that calls the action method on the player sent to the method
        private void PlayerAction(Player currentPlayer, GameBoard board)
        {
            currentPlayer.Action(currentPlayer.GetSpaceOccupied(), board, players);
        }
        
        //Method that rolls two dice and moves the player by there combined result
        private void MoveCurrentPlayerByDiceRoll()
        {
            int rollOne = die.GetDiceRoll();
            int rollTwo = die.GetDiceRoll();

            int playerPosition = players[currentPlayer].GetPlayerPosition();

            if (playerPosition + rollOne + rollTwo >= 40)
            {
                playerPosition -= 40;
                playerPosition += (rollTwo + rollOne);
                players[currentPlayer].AddMoney(GoSpace.MONEY_TO_COLLECT);
            }
            else
            {
                playerPosition += (rollTwo + rollOne);
            }

            players[currentPlayer].MovePlayerToSpace(board.GetSpaceById(playerPosition));
            Console.WriteLine("Player " + currentPlayer + " Roll - " + (rollOne + rollTwo) + " " + playerPosition);
        }

        //Returns a player object that matches the id sent. 
        //If the id doesn't match any player, null is return.
        public Player GetPlayerById(int id)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].GetPlayerId() == id)
                    return players[i];
            }

            return null;
        }

        //Tells server that player is disconnecting and whether if the player lost or won.
        private void GameCleanup()
        {
            if (players.Count == 4 && clientPlayerId != -1)
            {
                if (players[clientPlayerId].isBankrupt)
                    TellServerToDisconnectPlayer();
                else
                    TellServerPlayerWon();
            }

        }

        //Form method that activates when form is closing
        //Tells client cleanup if the player list isn't since cleanup relies on players.
        //Also re displays main menu
        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (players != null)
            {
                GameCleanup();
            }

            prevFrom.Show();
        }

        //Used by InputServerIp form for sending ipAddress between the two forms.
        //Sets the ip address.
        public void setIpAddress(string ipAddress)
        {
            this.ipAddress = ipAddress;
        }

        //Used by InputServerIp form for sending player color between the two forms.
        //Sets the player color that will be used in game.
        public void setPlayerColor(Color playerColor)
        {
            this.playerColor = playerColor;
        }
 

        private void GameForm_MouseMove(object sender, MouseEventArgs e)
        {


        }

        private void GameForm_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void GameForm_MouseDown(object sender, MouseEventArgs e)
        {

        }
    }
}
