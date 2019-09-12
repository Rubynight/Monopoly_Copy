using System;
using System.Collections.Generic;
using Monopoly.Lib;
using System.Drawing;
using System.IO;

namespace Monopoly.Board
{
    //This class is not sent over sockets, only the spaces are.
    public class GameBoard : RectangleObject
    {
        //Space ids for specific board spaces.
        //This spaces will always haves these ids
        private const int GO_SPACE_ID = 0;
        private const int JAIL_SPACE_ID = 10;
        private const int FREE_PARKING_SPACE_ID = 20;
        private const int GO_TO_JAIL_SPACE_ID = 30;

        //Properties
        private List<Space> boardSpaces;
        private const string fileLocation = @"..\..\..\Resources\Board\space.txt";
        private const String chanceLocation = @"..\..\..\Resources\Board\chancefile.txt";
        private StreamReader reader;
        private RectangleObject innerBoard;

        //Constructor
        public GameBoard(int x, int y, Color color, Size size) : base(x, y, color, size)
        {
            boardSpaces = new List<Space>();
            GenerateBoard();
        }

        //Method that calls methods that generates the board.
        //In this case, we are only generating the board space by laoding theme from
        //a file.
        public void GenerateBoard()
        {
            LoadBoardFromFile();
        }

        //Returns the a list of the spaces that make up the board.
        public List<Space> GetBoardSpaces()
        {
            return boardSpaces;
        }

        //Sets the spaces to a new list of board spaces
        //Used to updated all spaces on a board
        public void SetBoardSpaces(List<Space> spaces)
        {
            boardSpaces = spaces;
        }

        //Loops through all spaces and check for a matching id.
        //Returns space if the id matches, else it returns null
        public Space GetSpaceById(int id)
        {
            for (int i = 0; i < boardSpaces.Count; i++)
            {
                if (boardSpaces[i].GetSpaceId() == id)
                    return boardSpaces[i];
            }

            return null;
        }

        //Loops through all spaces and check for a matching name.
        //Returns space if the name matches, else it returns null
        public Space GetSpaceByName(string name)
        {
            for (int i = 0; i < boardSpaces.Count; i++)
            {
                if (boardSpaces[i] is PropertySpace p)
                {
                    if (p.GetProperty().GetPropertyName().Trim() == name)
                        return boardSpaces[i];
                }
            }

            return null;
        }

        //Finds the closest utility space from a staring location on the board and returns it.
        //If no utility was founds, it returns the first utiliy space on the board.
        public Space GetClosestUtility(int start)
        {
            for (int i = 0; i < boardSpaces.Count; i++)
            {
                if (boardSpaces[i] is UtilitySpace u)
                {
                    return boardSpaces[i];
                }
            }

            return GetSpaceById(4);
        }

        //Finds the closest railroad space from a staring location on the board and returns it.
        //If no railroad was founds, it returns the first railroad space on the board.
        public Space GetClosestRailroad(int start)
        {
            for (int i = 0; i < boardSpaces.Count; i++)
            {
                if (boardSpaces[i] is RailroadSpace r)
                {
                    return boardSpaces[i];
                }
            }

            return GetSpaceById(5);
        }

        //Returns the total amount of houses from all spaces.
        //Used for the effect of a chance card
        public int GetTotalNumOfHouse()
        {
            int total = 0;

            for (int i = 0; i < boardSpaces.Count; i++)
            {
                if (boardSpaces[i] is PropertySpace p)
                {
                    total += p.GetProperty().GetNumOfHouses();
                }
            }

            return total;
        }


        //Reads text file and generates board spaces.
        //Creates 40 spaces
        public void LoadBoardFromFile()
        {
            //Determines the dimensions of the spaces and the starting
            //locations for the four board lanes.
            double sizeMultiplier = FormatManager.GetSizeMultiplier();
            int SpaceSize = (int)((size.Width / 11) * sizeMultiplier);
            int LaneOneY = SpaceSize * 9 + SpaceSize;
            int LaneTwoX = 0;
            int LaneThreeY = 0;
            int LaneFourX = SpaceSize * 9 + SpaceSize;

            //Variables for reading the text file
            int i = 1;
            int x = 0;
            int id;
            String spaceType;
            String line;
            String[] lineSplit;
            char token = ',';
            reader = new StreamReader(fileLocation);

            //Add the four constant spaces and the inner board (rectangle within board).
            innerBoard = new RectangleObject(LaneTwoX + SpaceSize, LaneThreeY + SpaceSize, new Size(SpaceSize * 9, SpaceSize * 9));
            boardSpaces.Add(new JailSpace(JAIL_SPACE_ID, LaneOneY, LaneOneY, Color.White, new Size(SpaceSize, SpaceSize), RotateEnum.DoNotRotate));
            boardSpaces.Add(new FreeParkingSpace(FREE_PARKING_SPACE_ID, LaneTwoX, LaneOneY, Color.White, new Size(SpaceSize, SpaceSize), RotateEnum.Rotate270));
            boardSpaces.Add(new GoToJailSpace(GO_TO_JAIL_SPACE_ID, LaneTwoX, LaneThreeY, Color.White, new Size(SpaceSize, SpaceSize), RotateEnum.Rotate180));
            boardSpaces.Add(new GoSpace(GO_SPACE_ID, LaneFourX, LaneThreeY, Color.White, new Size(SpaceSize, SpaceSize), RotateEnum.Rotate90));

            //Reads the whole file and generates a board space for each line
            while (!reader.EndOfStream)
            {
                line = reader.ReadLine();
                lineSplit = line.Split(token);
                spaceType = lineSplit[0];

                //Moves to next lane once the lane is filled
                if (i == 10)
                {
                    i = 1;
                    x++;
                }

                //Keeps the id consistent across lanes
                id = (i + (x * 10));

                //Used to determine what kind of board space to create
                //Each case is a space type and each if is a lane
                switch (spaceType)
                {
                    case "Property":

                        Property property = new Property(lineSplit[1], int.Parse(lineSplit[2]), 0, Color.FromName(lineSplit[3].Trim()));
                        if (x == 0)
                            boardSpaces.Add(new PropertySpace(id, LaneFourX, (i * SpaceSize) + (SpaceSize - SpaceSize), property, new Size(SpaceSize, SpaceSize), RotateEnum.Rotate90));
                        else if (x == 1)
                            boardSpaces.Add(new PropertySpace(id, ((10 - i) * SpaceSize) + (SpaceSize - SpaceSize), LaneOneY, property, new Size(SpaceSize, SpaceSize), RotateEnum.DoNotRotate));
                        else if (x == 2)
                            boardSpaces.Add(new PropertySpace(id, LaneTwoX, ((10 - i) * SpaceSize) + (SpaceSize - SpaceSize), property, new Size(SpaceSize, SpaceSize), RotateEnum.Rotate270));
                        else
                            boardSpaces.Add(new PropertySpace(id, (i * SpaceSize) + (SpaceSize - SpaceSize), LaneThreeY, property, new Size(SpaceSize, SpaceSize), RotateEnum.Rotate180));
                        break;
                    case "Railroad":
                        Railroad railroad = new Railroad(lineSplit[1], int.Parse(lineSplit[2]));
                        if (x == 0)
                            boardSpaces.Add(new RailroadSpace(id, LaneFourX, (i * SpaceSize) + (SpaceSize - SpaceSize), railroad, new Size(SpaceSize, SpaceSize), RotateEnum.Rotate90));
                        else if (x == 1)
                            boardSpaces.Add(new RailroadSpace(id, ((10 - i) * SpaceSize) + (SpaceSize - SpaceSize), LaneOneY, railroad, new Size(SpaceSize, SpaceSize), RotateEnum.DoNotRotate));
                        else if (x == 2)
                            boardSpaces.Add(new RailroadSpace(id, LaneTwoX, ((10 - i) * SpaceSize) + (SpaceSize - SpaceSize), railroad, new Size(SpaceSize, SpaceSize), RotateEnum.Rotate270));
                        else
                            boardSpaces.Add(new RailroadSpace(id, (i * SpaceSize) + (SpaceSize - SpaceSize), LaneThreeY, railroad, new Size(SpaceSize, SpaceSize), RotateEnum.Rotate180));
                        break;
                    case "CommunityChest":
                        if (x == 0)
                            boardSpaces.Add(new CommunityChestSpace(id, LaneFourX, (i * SpaceSize) + (SpaceSize - SpaceSize), new Size(SpaceSize, SpaceSize), RotateEnum.Rotate90));
                        else if (x == 1)
                            boardSpaces.Add(new CommunityChestSpace(id, ((10 - i) * SpaceSize) + (SpaceSize - SpaceSize), LaneOneY, new Size(SpaceSize, SpaceSize), RotateEnum.DoNotRotate));
                        else if (x == 0)
                            boardSpaces.Add(new CommunityChestSpace(id, LaneTwoX, ((10 - i) * SpaceSize) + (SpaceSize - SpaceSize), new Size(SpaceSize, SpaceSize), RotateEnum.Rotate270));
                        else
                            boardSpaces.Add(new CommunityChestSpace(id, (i * SpaceSize) + (SpaceSize - SpaceSize), LaneThreeY, new Size(SpaceSize, SpaceSize), RotateEnum.Rotate180));
                        break;
                    case "Chance":
                        if (x == 0)
                            boardSpaces.Add(new ChanceSpace(id, LaneFourX, (i * SpaceSize) + (SpaceSize - SpaceSize), new Size(SpaceSize, SpaceSize), RotateEnum.Rotate90));
                        else if (x == 1)
                            boardSpaces.Add(new ChanceSpace(id, ((10 - i) * SpaceSize) + (SpaceSize - SpaceSize), LaneOneY, new Size(SpaceSize, SpaceSize), RotateEnum.DoNotRotate));
                        else if (x == 2)
                            boardSpaces.Add(new ChanceSpace(id, LaneTwoX, ((10 - i) * SpaceSize) + (SpaceSize - SpaceSize), new Size(SpaceSize, SpaceSize), RotateEnum.Rotate270));
                        else
                            boardSpaces.Add(new ChanceSpace(id, (i * SpaceSize) + (SpaceSize - SpaceSize), LaneThreeY, new Size(SpaceSize, SpaceSize), RotateEnum.Rotate180));
                        break;
                    case "Utility":
                        Utility utility = new Utility(lineSplit[1], 100);
                        if (x == 0)
                            boardSpaces.Add(new UtilitySpace(id, LaneFourX, (i * SpaceSize) + (SpaceSize - SpaceSize), utility, new Size(SpaceSize, SpaceSize), RotateEnum.Rotate90));
                        else if (x == 1)
                            boardSpaces.Add(new UtilitySpace(id, ((10 - i) * SpaceSize) + (SpaceSize - SpaceSize), LaneOneY, utility, new Size(SpaceSize, SpaceSize), RotateEnum.DoNotRotate));
                        else if (x == 2)
                            boardSpaces.Add(new UtilitySpace(id, LaneTwoX, ((10 - i) * SpaceSize) + (SpaceSize - SpaceSize), utility, new Size(SpaceSize, SpaceSize), RotateEnum.Rotate270));
                        else
                            boardSpaces.Add(new UtilitySpace(id, (i * SpaceSize) + (SpaceSize - SpaceSize), LaneThreeY, utility, new Size(SpaceSize, SpaceSize), RotateEnum.Rotate180));
                        break;
                }

                i++;
            }

            reader.Close();
        }

        //Returns the Go Space
        public Space GetStartingSpace()
        {
            return GetSpaceById(GO_SPACE_ID);
        }

        //Returns the Jail Space
        public Space GetJailSpace()
        {
            return GetSpaceById(JAIL_SPACE_ID);
        }

        //Draws the inner rectangle of the boad with respect to the camera
        public override void Draw(Graphics g, int xOffset, int yOffset)
        {
            base.Draw(g, xOffset, yOffset);
            g.FillRectangle(Brushes.Purple, new Rectangle(innerBoard.GetX() + xOffset, innerBoard.GetY() + yOffset, innerBoard.GetWidth(), innerBoard.GetHeight()));
        }
    }
}
