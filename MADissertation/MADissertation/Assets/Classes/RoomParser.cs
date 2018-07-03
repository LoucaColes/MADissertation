using System.Collections;
using System.Collections.Generic;
using LevelGeneration;
using UnityEngine;

public static class RoomParser
{
    public static SimpleRoomData ParseRoomToString(Room _room)
    {
        SimpleRoomData roomData = new SimpleRoomData();
        roomData.m_xPos = _room.GetPosition().x;
        roomData.m_yPos = _room.GetPosition().y;
        RoomType type = _room.GetRoomType();
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
        return roomData;
    }
}