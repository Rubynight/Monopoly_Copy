using Monoploy.Board;
using Monopoly.Lib;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;

namespace Monopoly.Board
{
    public class ChanceSpace : Space
    {
        //Properties
        public const string text = "Chance";
        public static Stack<ChanceCards> chanceCards;
        public static Shuffler<ChanceCards> shuffler;


        //Constructor that sets the text format and rotation orientation for the chance space.
        //Calls the base constructor (Space) to create the geometry for the chance space.
        //This constructor is called when deserailizing json strings.
        [JsonConstructor]
        public ChanceSpace(int spaceId, int x, int y, Size size, RotateEnum rotate)
            : base(spaceId, x, y, size, rotate)
        {
            textFormat = new StringFormat();
            textFormat.LineAlignment = StringAlignment.Far;
            textFormat.Alignment = StringAlignment.Center;

            if (rotate == RotateEnum.Rotate90 || rotate == RotateEnum.Rotate270)
                Rotate();

            chanceCards = new Stack<ChanceCards>();
            LoadCards();
        }

        //Creates a shuffler object and shuffles the list of Chance Cards.
        //The shuffle list is than stored in a stack for use when drawing the cards.
        public static void LoadCards()
        {
            shuffler = new Shuffler<ChanceCards>(ChanceCards.LoadChanceFromFile());
            List<ChanceCards> cardsToStack= shuffler.GetNewShuffle();

            foreach (ChanceCards card in cardsToStack)
                chanceCards.Push(card);
        }

        //Returns the number of chance cards remaining in the stack.
        //Used to determine when to re-shuffle the cards.
        public static int GetAmountOfCardsLeft()
        {
            return chanceCards.Count;
        }

        //Pops the chance card stack and returns it.
        //When the stack is empty, it reloads the cards and continue.
        public static ChanceCards DrawCard()
        {
            if (chanceCards.Count == 0)
            {
                LoadCards();
            }
            
            return chanceCards.Pop();
        }

        //Renders the chance space text
        //Calls the parent class Draw to render the actually space.
        //The space is render with respect to the camera's viewport
        public override void Draw(Graphics g, int xOffset, int yOffset)
        {
            base.Draw(g, xOffset, yOffset);

            RotateDrawText(g, text, rectangle, font, textColor, textFormat);
        }
    }
}
