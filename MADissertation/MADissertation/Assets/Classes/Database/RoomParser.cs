using LevelGeneration;

/// <summary>
/// A parser that can convert the room class into other formats
/// </summary>
public static class RoomParser
{
    /// <summary>
    /// Converts the Room class to a string of data
    /// </summary>
    /// <param name="_room">Room to parse</param>
    /// <returns>String of simple data</returns>
    public static SimpleRoomData ParseRoomToString(Room _room)
    {
        // Create a new simple room data
        SimpleRoomData roomData = new SimpleRoomData();

        // Set the simple room data's positions
        roomData.m_xPos = _room.GetPosition().x;
        roomData.m_yPos = _room.GetPosition().y;

        // Get the room type
        RoomType type = _room.GetRoomType();

        // Check the room type and set the letter
        // Representative based on the type
        if (type == RoomType.Empty)
        {
            roomData.m_letterRepresentative = "E_";
        }
        if (type == RoomType.Start)
        {
            roomData.m_letterRepresentative = "S_";
        }
        if (type == RoomType.End)
        {
            roomData.m_letterRepresentative = "G_";
        }
        if (type == RoomType.Path)
        {
            roomData.m_letterRepresentative = "P_";
        }

        // Set the simple room data's name
        roomData.m_prefabName = _room.gameObject.name;

        // return the parsed data
        return roomData;
    }
}