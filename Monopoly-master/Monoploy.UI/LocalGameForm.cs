using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Monopoly.Lib;
using Monopoly.Board;
/*
* Game form used for when player is playing a local game. 
* Doesn't contain any method related to socket server messages and
* allows players to sell properties.
*/

namespace Monoploy.UI
{
    public partial class LocalGameForm : Form
    {
        private List<GameObject> gameObjects;
        private GameBoard board;
        private Camera camera;
        private int totalTimeElapsed;
        private GameTimer timer;
        private int windowWidth;
        private int windowHeight;
        private bool isDragging;
        private List<Player> players;
        private Die die;
        private int currentPlayer;
        private GameObject clickedObject;

        public LocalGameForm()
        {
            //Init Form, should always be the first command executed. SetStyle is rendering hints to tell the
            //form how to draw. These hints will prevent flickering.
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            Rectangle screen = Screen.PrimaryScreen.Bounds;
            windowHeight = screen.Height;
            windowWidth = screen.Width;

            //Init objects
            gameObjects = new List<GameObject>();
            camera = new Camera(windowWidth, windowHeight, 0, 0);
            die = new Die();
            timer = new GameTimer();
            isDragging = false;

            //Calls collection of methods that setup components of the game.
            SetupGame(SizeEnum.Large);

        }

        private void SetupGame(SizeEnum size)
        {
            FormatManager.SetupFormatManager(size);
            SetupBoard(size);
            totalTimeElapsed = 0;
            timer.SetupGameTimer(RecordTime);
            camera.centerOnGameObject(gameObjects[4]);

            //Generate players
            players = new List<Player>(1)
            {
                new Player(0, board.GetStartingSpace(), Color.Red),
                new Player(1, board.GetStartingSpace(), Color.Blue),
                new Player(2, board.GetStartingSpace(), Color.Green),
                new Player(3, board.GetStartingSpace(), Color.Yellow)
            };
            currentPlayer = 0;
            gameObjects.AddRange(players);
        }

        //Creates a new board and adds it and its spaces to the gameobjects list.
        private void SetupBoard(SizeEnum size)
        {
            board = new GameBoard(0, 0, Color.Blue, new Size(2048, 2048));
            gameObjects.Add(board);
            gameObjects.AddRange(board.GetBoardSpaces());
        }

        /*
         * GameTimer method that records how long
         * the game has been played.
         */
        public void RecordTime(object sender, EventArgs e)
        {
            totalTimeElapsed += 1;
        }

        //Form method that runs when refresh is called. Also provies the graphic object that is
        //used to draw on the form
        private void GameForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (gameObjects != null)
            {

                foreach (GameObject gameObject in gameObjects)
                {
                    if (gameObject is Player temp)
                    {
                        temp = (Player)gameObject;
                        temp.Draw(g, temp.AdjustForXOffset(camera.GetXOffset()), temp.AdjustForYOffset(camera.GetYOffset()), currentPlayer);
                    }
                    else
                    {
                        gameObject.Draw(g, gameObject.AdjustForXOffset(camera.GetXOffset()), gameObject.AdjustForYOffset(camera.GetYOffset()));

                        if (clickedObject is IClickToDrawText clickToDrawText)
                        {
                            g.DrawString(clickToDrawText.TextToDraw(), FormatManager.GetPlayerFont(), new SolidBrush(Color.Black), new PointF(0, windowHeight - FormatManager.GetPlayerFont().Height));
                        }
                    }
                }
            }
        }

        //Form method activates when a user clicks on the form. It provides mouse_event_args which gives
        //the x and y location of the mouse. 
        //This method is used to determine if a object was clicked and if so, center on it and call any related methods.
        private void GameForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (gameObjects != null && !isDragging)
            {
                foreach (GameObject gameObject in gameObjects)
                {
                    if (gameObject.WasClicked(e.X, e.Y, gameObject.AdjustForXOffset(camera.GetXOffset()), gameObject.AdjustForYOffset(camera.GetYOffset())))
                    {
                        clickedObject = gameObject;
                        gameObject.ClickAction();
                        camera.centerOnGameObject(gameObject);
                        break;
                    }
                }

            }

            this.Refresh();
        }

        //Sets the form style to borderless and maximized
        private void GameForm_Load(object sender, EventArgs e)
        {
            this.ControlBox = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
        }

        //Form method that activates when a key is press. Used to perfrom key relate actions in the game, such
        //as rolling the dice, moving the camera, etc.
        private void GameForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();

            if (e.KeyCode == Keys.Up)
                camera.move(0, -Camera.CAMERA_SPEED);

            if (e.KeyCode == Keys.Down)
                camera.move(0, Camera.CAMERA_SPEED);

            if (e.KeyCode == Keys.Left)
                camera.move(-Camera.CAMERA_SPEED, 0);

            if (e.KeyCode == Keys.Right)
                camera.move(Camera.CAMERA_SPEED, 0);

            if (e.KeyCode == Keys.R)
            {
                if (!players[currentPlayer].IsPlayerBankrupt())
                    MoveCurrentPlayer();

                clickedObject = null;
                this.Refresh();
                if (CheckIfPlayerWon())
                {
                    MessageBox.Show("Player " + currentPlayer + " has won");
                    Close();
                }
            }

            //Debug command
            if (e.KeyCode == Keys.U)
            {
                foreach (GameObject g in gameObjects)
                {
                    if (g is PropertySpace p)
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

            //Used to sell properties
            if (e.KeyCode == Keys.B && players[currentPlayer].GetSpaceOccupied() is PropertySpace)
            {
                Player player = players[currentPlayer];
                PropertySpace property = (PropertySpace)player.GetSpaceOccupied();

                if (property.GetProperty().GetOwner() == player)
                {
                    using (SellProperty sellProperty = new SellProperty(property.GetProperty(), players, player))
                    {
                        sellProperty.ShowDialog();
                    }
                }

                this.Refresh();
            }

            this.Refresh();
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

            //Makes sure that player loops around the board
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

        private void GameForm_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void GameForm_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void GameForm_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
