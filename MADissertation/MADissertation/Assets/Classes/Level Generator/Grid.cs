using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Level Generation specific namespace
/// </summary>
namespace LevelGeneration
{
    /// <summary>
    /// Grid class that is used to generate the levels
    /// </summary>
    public class Grid : MonoBehaviour
    {
        // Private variables
        private int m_gridWidth;
        private int m_gridHeight;
        private Vector2 m_offset;
        private List<Room> m_rooms;
        private List<Room> m_path;
        private TemplateHolder m_templateHolder;
        private CinemachineSmoothPath m_dolly;
        private CinemachineVirtualCamera m_dollyCam;
        private GameObject m_playerPref;
        private int m_collectableCount = 0;

        /// <summary>
        /// Create a grid of empty rooms
        /// </summary>
        /// <param name="_gridWidth">Grid Width</param>
        /// <param name="_gridHeight">Grid Height</param>
        /// <param name="_emptyRoom">Empty Room Prefab</param>
        /// <param name="_offset">Position Offset</param>
        /// <param name="_templateHolder">Template Holder Scriptable Object</param>
        /// <param name="_player">Player Prefab</param>
        public void CreateGrid(int _gridWidth, int _gridHeight, GameObject _emptyRoom, Vector2 _offset,
            TemplateHolder _templateHolder, GameObject _player)
        {
            // Set the grid with and height
            m_gridWidth = _gridWidth;
            m_gridHeight = _gridHeight;

            // Calculate the grid size
            int gridSize = _gridWidth * _gridHeight;

            // Set the positional offset
            m_offset = _offset;

            // Create a new list of rooms using the grid size
            m_rooms = new List<Room>(gridSize);

            // Loop through and create an empty room at each grid position
            for (int i = 0; i < _gridHeight; i++)
            {
                for (int j = 0; j < _gridWidth; j++)
                {
                    // Calculate the offset position
                    Vector3 position = new Vector3(j * m_offset.x, i * m_offset.y, 0);

                    // Create an empty room
                    GameObject newRoomObject = (GameObject)Instantiate(_emptyRoom, position, Quaternion.identity);

                    // Parent room to the grid object
                    newRoomObject.transform.parent = this.transform;

                    // Add the grid co ordinates to the new room's name
                    newRoomObject.name += "_" + j + "_" + i;

                    // Get the room class
                    Room room = newRoomObject.GetComponent<Room>();

                    // Create a integer version of the rooms position
                    Vector2Int intPosition = new Vector2Int(j, i);

                    // Initialise the room
                    room.Init(RoomType.Empty, intPosition);

                    // Add the room to the list
                    m_rooms.Add(room);
                }
            }

            // Set the template holer
            m_templateHolder = _templateHolder;

            // Find the dolly points object
            m_dolly = GameObject.FindObjectOfType<CinemachineSmoothPath>();

            // Find the dolly camera object
            m_dollyCam = GameObject.FindObjectOfType<CinemachineVirtualCamera>();

            // Set the player prefab
            m_playerPref = _player;
        }

        /// <summary>
        /// Generate a level
        /// </summary>
        public void GenerateLevel()
        {
            // Clear grid
            Clear();

            // Create path
            m_path = new List<Room>();
            GeneratePath(out m_path);

            // Init start room
            m_path[0].SetRoomType(RoomType.Start);
            Vector3 startRoomPostion = m_path[0].transform.position;

            // Move the camera to the room's position
            Vector3 newCameraPosition = new Vector3(startRoomPostion.x, startRoomPostion.y, -10);
            m_dolly.gameObject.transform.position = newCameraPosition;
            m_dollyCam.gameObject.transform.position = newCameraPosition;
            m_dolly.gameObject.transform.position = new Vector3(0, 0, -10);

            // Init end room
            m_path[m_path.Count - 1].SetRoomType(RoomType.End);

            // Replace blank rooms
            ReplaceAllRooms();

            // Set up connections
            SetUpRoomConnections();

            // Set up end room end zone
            m_path[m_path.Count - 1].EnableEndZone();

            // Spawn player at the start room
            Vector3 spawnPosition = m_path[0].GetSpawnPosition().position;
            GameObject player = (GameObject)Instantiate(m_playerPref, spawnPosition, Quaternion.identity);

            // Set the follow and look at field to the player transform
            m_dollyCam.m_Follow = player.transform;
            m_dollyCam.m_LookAt = player.transform;

            // Create a new array of waypoints equal to the size of the path list
            CinemachineSmoothPath.Waypoint[] waypoints = new CinemachineSmoothPath.Waypoint[m_path.Count];

            // Loop through and set each waypoints position
            for (int i = 0; i < m_path.Count; i++)
            {
                waypoints[i].position = m_path[i].transform.position;
            }

            // Set the dolly's waypoints to the new waypoints
            m_dolly.m_Waypoints = waypoints;

            // Set the dolly points and camera's position to the start room position
            m_dolly.gameObject.transform.position = new Vector3(startRoomPostion.x, startRoomPostion.y, -7.5f);
            m_dollyCam.transform.position = m_dolly.gameObject.transform.position;

            // Get the player class
            Player playerClass = player.GetComponent<Player>();

            // Set the player's spawn position
            playerClass.SetSpawn(m_path[0].GetSpawnPosition().position);

            // Reset the player
            playerClass.ResetPlayer();

            // Set the player's current room to the start room
            playerClass.SetCurrentRoom(m_path[0]);

            // Set up level data
            DesignData currentLevelData = new DesignData();
            currentLevelData.m_gridWidth = m_gridWidth;
            currentLevelData.m_gridHeight = m_gridHeight;
            currentLevelData.m_grid = m_rooms.ToArray();

            // Create a list of simple room data
            List<SimpleRoomData> roomData = new List<SimpleRoomData>();

            // Loop through each room and parse it to receive a simplier version
            foreach (Room room in m_rooms)
            {
                roomData.Add(RoomParser.ParseRoomToString(room));
            }

            // Set the simple room data in the current level data
            currentLevelData.m_simpleRoomData = roomData;

            // Remove any empty rooms
            RemoveEmptyRooms();

            // Pass current level data to the data tracker
            DataTracker.Instance.AddNewLevelData(ref currentLevelData);

            // Set the dolly points and camera to the start rooms position
            m_dolly.gameObject.transform.position = new Vector3(0, 0, -7.5f);
            m_dollyCam.transform.position = new Vector3(startRoomPostion.x, startRoomPostion.y, -7.5f);
        }

        /// <summary>
        /// Generate a path through the level
        /// </summary>
        /// <param name="path">Path of rooms</param>
        private void GeneratePath(out List<Room> path)
        {
            // Create a new list of rooms
            path = new List<Room>();

            // Generate start room
            Vector2Int startRoomPos = new Vector2Int(UnityEngine.Random.Range(0, m_gridWidth), 0);

            // Decide whether to move left or right
            int xPos = startRoomPos.x;
            int yPos = 0;

            // While path has not reached the top
            while (yPos < m_gridHeight)
            {
                // Get room at set position
                Room room = GetRoom(xPos, yPos);
                room.SetRoomType(RoomType.Path); path.Add(room);

                // Generate random number
                int randLeftRight = UnityEngine.Random.Range(0, 100);
                // Move left if less than 50
                if (randLeftRight < 50)
                {
                    // Check if edge
                    if (xPos == 0)
                    {
                        // Move up by one
                        yPos++;
                    }
                    else // If not edge
                    {
                        // Set diffence of the edge and the x position
                        int difference = xPos;

                        // Calculate move amount
                        int moveAmount = UnityEngine.Random.Range(1, difference);

                        // Move to the left for the required amount
                        for (int moveIndex = (xPos - 1); moveIndex >= (xPos - moveAmount); moveIndex--)
                        {
                            if (moveIndex >= 0)
                            {
                                // Add room to the path
                                Room newRoom = GetRoom(moveIndex, yPos);
                                newRoom.SetRoomType(RoomType.Path);
                                path.Add(newRoom);
                            }
                        }

                        // Update the x position
                        xPos -= moveAmount;

                        // Increase the y position
                        yPos++;
                    }
                }
                else // move right
                {
                    // check if edge
                    if (xPos == (m_gridWidth - 1))
                    {
                        // Move up by one
                        yPos++;
                    }
                    else
                    {
                        // Set the diffence of the edge and the x position
                        int difference = (m_gridWidth - 1) - xPos;

                        // Calculate move amount
                        int moveAmount = UnityEngine.Random.Range(1, difference);

                        // Move to the left for the required amount
                        for (int moveIndex = (xPos + 1); moveIndex <= (xPos + moveAmount); moveIndex++)
                        {
                            if (moveIndex <= m_gridWidth - 1)
                            {
                                // Add room to the path
                                Room newRoom = GetRoom(moveIndex, yPos);
                                newRoom.SetRoomType(RoomType.Path);
                                path.Add(newRoom);
                            }
                        }

                        // Update the x position
                        xPos += moveAmount;

                        // Increase the y position by one
                        yPos++;
                    }
                }
            }
        }

        /// <summary>
        /// Replace all empty rooms on path with templated rooms
        /// </summary>
        private void ReplaceAllRooms()
        {
            // Loop through the path
            for (int roomIndex = 0; roomIndex < m_path.Count; roomIndex++)
            {
                // Store the current room in the path
                Room currentRoom = m_path[roomIndex];

                // Store the current rooms position
                Vector3 roomPosition = currentRoom.gameObject.transform.position;

                // Create a new room
                GameObject newRoomObject;

                // Figure out which template group to use based on the difficulty
                TemplateGroup group = m_templateHolder.GeTemplateGroup(DataTracker.Instance.GetDifficulty());

                // Spawn a new room using that template groups prefabs
                newRoomObject = (GameObject)Instantiate(group.m_roomPrefabs[UnityEngine.Random.Range(0,
                            group.m_roomPrefabs.Length)],
                    roomPosition, Quaternion.identity);

                // Parent the new room to the grid
                newRoomObject.transform.parent = this.transform;

                // Add the positions to the name of the new room
                newRoomObject.name += "_" + currentRoom.GetPosition().x + "_" + currentRoom.GetPosition().y;

                // Initialise the new room and attempt to spawn a collectable
                Room replacementRoom = newRoomObject.GetComponent<Room>();
                replacementRoom.Init(currentRoom.GetRoomType(), currentRoom.GetPosition());
                replacementRoom.AttemptSpawnCollectable(ref m_collectableCount, group.m_collectableCount);

                // Remove the old room and then replace with the new room in the path list
                m_path.RemoveAt(roomIndex);
                m_path.Insert(roomIndex, replacementRoom);

                // Remove the old room and then replace with the new room in the rooms list
                int indexPosition = m_rooms.IndexOf(currentRoom);
                m_rooms.RemoveAt(indexPosition);
                m_rooms.Insert(indexPosition, replacementRoom);

                // Destroy the old room's game object
                Destroy(currentRoom.gameObject);
            }
        }

        /// <summary>
        /// Set up connections between rooms
        /// </summary>
        private void SetUpRoomConnections()
        {
            // Loop through all the rooms in the path
            for (int roomIndex = 0; roomIndex < m_path.Count; roomIndex++)
            {
                // If start room only check next
                if (roomIndex == 0)
                {
                    // Get the current room
                    Room currentRoom = m_path[roomIndex];

                    // Figure out the next index
                    int nextIndex = roomIndex + 1;

                    // Get the next room
                    Room nextRoom = m_path[nextIndex];

                    // Create a new connection string and check the two rooms for a connection
                    string connection;
                    CheckRoomConnections(nextRoom, currentRoom, out connection);

                    // Parse the string to get a ConnectionType enum
                    ConnectionType connectionType = (ConnectionType)Enum.Parse(typeof(ConnectionType), connection);

                    // Set the current rooms connection type and link the two rooms
                    currentRoom.SetConnectionType(connectionType);
                    currentRoom.LinkRooms(connection, nextRoom);
                }
                else if (roomIndex == (m_path.Count - 1)) // If end room only check previous
                {
                    // Get the current room
                    Room currentRoom = m_path[roomIndex];

                    // Figure out the previous index
                    int prevIndex = roomIndex - 1;

                    // Get the previous room
                    Room prevRoom = m_path[prevIndex];

                    // Create a new connection string and check the two rooms for a connection
                    string connection;
                    CheckRoomConnections(prevRoom, currentRoom, out connection);

                    // Parse the string to get a ConnectionType enum
                    ConnectionType connectionType = (ConnectionType)Enum.Parse(typeof(ConnectionType), connection);

                    // Set the current rooms connection type and link the two rooms
                    currentRoom.SetConnectionType(connectionType);
                    currentRoom.LinkRooms(connection, prevRoom);
                }
                else // Else check next and previous
                {
                    // Get the current room
                    Room currentRoom = m_path[roomIndex];

                    // Figure out the next index
                    int nextIndex = roomIndex + 1;

                    // Get the next room
                    Room nextRoom = m_path[nextIndex];

                    // Create a new connection string and check the two rooms for a connection
                    string connectionOne;
                    CheckRoomConnections(nextRoom, currentRoom, out connectionOne);

                    // Link the two rooms
                    currentRoom.LinkRooms(connectionOne, nextRoom);

                    // Figure out the previous index
                    int prevIndex = roomIndex - 1;

                    // Get the previous room
                    Room prevRoom = m_path[prevIndex];

                    // Create a new connection string and check the two rooms for a connection
                    string connectionTwo;
                    CheckRoomConnections(prevRoom, currentRoom, out connectionTwo);

                    // Link the two rooms
                    currentRoom.LinkRooms(connectionTwo, prevRoom);

                    // Combine the two connections strings
                    string connection = connectionOne + connectionTwo;

                    // Parse the string to get a ConnectionType enum
                    ConnectionType connectionType = (ConnectionType)Enum.Parse(typeof(ConnectionType), connection);

                    // Set the current room's connection type
                    currentRoom.SetConnectionType(connectionType);
                }
            }
        }

        /// <summary>
        /// Check for connects between two rooms
        /// </summary>
        /// <param name="nextPrevRoom">The next or previous room</param>
        /// <param name="currentRoom">The current room</param>
        /// <param name="connection">Connection type</param>
        private void CheckRoomConnections(Room nextPrevRoom, Room currentRoom, out string connection)
        {
            // Check for left connection
            if (nextPrevRoom.GetPosition().x < currentRoom.GetPosition().x)
            {
                connection = "L";
            }
            // Check for right connection
            else if (nextPrevRoom.GetPosition().x > currentRoom.GetPosition().x)
            {
                connection = "R";
            }
            // Check for up connection
            else if (nextPrevRoom.GetPosition().y > currentRoom.GetPosition().y)
            {
                connection = "U";
            }
            // Check for down connection
            else
            {
                connection = "D";
            }
        }

        /// <summary>
        /// Get a specific room
        /// </summary>
        /// <param name="_x">X position</param>
        /// <param name="_y">Y position</param>
        /// <returns>Room matching the specific co ordinates</returns>
        public Room GetRoom(int _x, int _y)
        {
            return m_rooms[(int)((_y * m_gridWidth) + _x)];
        }

        /// <summary>
        /// Clear the grid
        /// </summary>
        public void Clear()
        {
            // Loop through each room, set to empty and remove any connections
            foreach (Room room in m_rooms)
            {
                room.SetRoomType(RoomType.Empty);
                room.SetConnectionType(ConnectionType.none);
            }
        }

        /// <summary>
        /// Remove any unwanted empty rooms
        /// </summary>
        private void RemoveEmptyRooms()
        {
            // Loop through each room in the rooms list
            foreach (Room room in m_rooms)
            {
                // Check if the room is empty
                if (room.GetRoomType() == RoomType.Empty)
                {
                    // If empty destroy the room
                    Destroy(room.gameObject);
                }
            }
        }

        /// <summary>
        /// Access the start room world position
        /// </summary>
        /// <returns>Returns the start rooms world position</returns>
        public Vector3 GetStartRoomPos()
        {
            return m_path[0].transform.position;
        }
    }
}