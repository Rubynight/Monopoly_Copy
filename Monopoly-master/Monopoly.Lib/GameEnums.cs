namespace Monopoly.Lib
{
    /*
     * Enums used for better identification  
     */

    //Size Enum for determining what size you want objects in the game to be
    public enum SizeEnum
    {
        Large,
        Medium,
        Small
    }


    //Rotate enum for determining what direction you want the object to be rendered in
    public enum RotateEnum
    {
        DoNotRotate = 0,
        Rotate90 = 270,
        Rotate180 = 180,
        Rotate270 = 90,
    }

    public enum MessageEnum
    {
        CreatePlayer,
        ClearBoard,
        ClearPlayers,
        GetPlayers,
        GetClientId,
        GetPropertySpaces,
        GetJailSpaces,
        GetCommunityChestSpaces,
        GetChanceSpaces,
        GetFreeParkingSpace,
        GetGoSpace,
        GetGoToJailSpace,
        GetRailroadSpace,
        GetUtilitySpaces,
        SetPlayers,
        SetClientId,
        SetPropertySpaces,
        SetJailSpaces,
        SetCommunityChestSpaces,
        SetChanceSpaces,
        SetFreeParkingSpace,
        SetGoSpace,
        SetGoToJailSpace,
        SetRailroadSpace,
        SetUtilitySpaces
    }
}
