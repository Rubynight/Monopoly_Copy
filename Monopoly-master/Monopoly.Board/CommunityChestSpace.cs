using Monopoly.Lib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;

namespace Monopoly.Board
{
    //[JsonObject(IsReference = true)]
    public class CommunityChestSpace : Space
    {
        //Properties
        public const string text = "Community Chest";
        public static Stack<CommunuityChestCards> chestCards { get; set; }
        public static Shuffler<CommunuityChestCards> shuffler { get; set; }

        //Constructor that sets the text format and rotation orientation for the community chest space.
        //Calls the base constructor (Space) to create the geometry for the community chest space.
        //This constructor is called when deserailizing json strings.
        [JsonConstructor]
        public CommunityChestSpace(int spaceId, int x, int y, Size size, RotateEnum rotate)
            : base(spaceId, x, y, size, rotate)
        {
            textFormat = new StringFormat();
            textFormat.LineAlignment = StringAlignment.Far;
            textFormat.Alignment = StringAlignment.Center;

            if (rotate == RotateEnum.Rotate90 || rotate == RotateEnum.Rotate270)
                Rotate();

            chestCards = new Stack<CommunuityChestCards>();
            GenerateQuadrants();
            LoadCards();
        }

        //Returns the number of community chest cards remaining in the stack.
        //Used to determine when to re-shuffle the cards.
        public static int GetAmountOfCardsLeft()
        {
            return chestCards.Count;
        }

        //Pops the community chest card stack and returns it.
        //When the stack is empty, it reloads the cards and continue.
        public static CommunuityChestCards DrawCard()
        {
            if (chestCards.Count == 0)
            {
                LoadCards();
            }

            return chestCards.Pop();
        }

        //Creates a shuffler object and shuffles the list of Community Chest Cards.
        //The shuffle list is than stored in a stack for use when drawing the cards.
        public static void LoadCards()
        {
            shuffler = new Shuffler<CommunuityChestCards>(CommunuityChestCards.LoadCommunityChestCardFromFile());
            List<CommunuityChestCards> cardsToStack = shuffler.GetNewShuffle();

            foreach (CommunuityChestCards card in cardsToStack)
                chestCards.Push(card);
        }

        //Renders the community chest space text
        //Calls the parent class Draw to render the actually space.
        //The space is render with respect to the camera's viewport
        public override void Draw(Graphics g, int xOffset, int yOffset)
        {
            base.Draw(g, xOffset, yOffset);

            RotateDrawText(g, text, rectangle, font, textColor, textFormat);


        }
    }
}
