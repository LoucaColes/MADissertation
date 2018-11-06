using LevelGeneration;
using UnityEngine;

/// <summary>
/// Used to help transition from room to room
/// </summary>
public class RoomExit : MonoBehaviour
{
    [SerializeField]
    private Room m_connectedRoom; // Room that the exit collider is connected to

    /// <summary>
    /// Set the connected room
    /// </summary>
    /// <param name="_room">Connected room</param>
    public void SetConnectedRoom(Room _room)
    {
        m_connectedRoom = _room;
    }

    /// <summary>
    /// Access the connected room
    /// </summary>
    /// <returns>Connected room</returns>
    public Room GetRoom()
    {
        return m_connectedRoom;
    }
}