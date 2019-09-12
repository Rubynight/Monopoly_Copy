namespace Monopoly.Board
{
    //Interface for telling classes to be able to render text on the screen.
    //Specifically the bottom left corner.
    public interface IClickToDrawText
    {
        string TextToDraw();
    }
}
