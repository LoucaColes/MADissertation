using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LevelGeneration
{
    public class Grid : MonoBehaviour
    {
        private int m_gridWidth;
        private int m_gridHeight;
        private Vector2 m_offset;
        private List<Room> m_rooms;
        private List<Room> m_path;
        private TemplateHolder m_templateHolder;
        private CameraMovement m_cameraMovement;
        private GameObject m_playerPref;

        public void CreateGrid(int _gridWidth, int _gridHeight, GameObject _emptyRoom, Vector2 _offset,
            TemplateHolder _templateHolder, CameraMovement _cameraMovement, GameObject _player)
        {
            m_gridWidth = _gridWidth;
            m_gridHeight = _gridHeight;
            int gridSize = _gridWidth * _gridHeight;
            m_offset = _offset;
            m_rooms = new List<Room>(gridSize);
            for (int i = 0; i < _gridHeight; i++)
            {
                for (int j = 0; j < _gridWidth; j++)
                {
                    Vector3 position = new Vector3(j * m_offset.x, i * m_offset.y, 0);
                    GameObject newRoomObject = (GameObject)Instantiate(_emptyRoom, position, Quaternion.identity);
                    newRoomObject.transform.parent = this.transform;
                    newRoomObject.name += "_" + j + "_" + i;
                    Room room = newRoomObject.GetComponent<Room>();
                    Vector2Int intPosition = new Vector2Int(j, i);
                    room.Init(RoomType.Empty, intPosition);
                    m_rooms.Add(room);
                }
            }

            m_templateHolder = _templateHolder;
            m_cameraMovement = _cameraMovement;
            m_playerPref = _player;
        }

        public void GenerateLevel()
        {
            // clear grid
            Clear();

            // create path
            m_path = new List<Room>();
            GeneratePath(out m_path);

            // init start room
            m_path[0].SetRoomType(RoomType.Start);
            Vector3 startRoomPostion = m_path[0].transform.position;
            Vector3 newCameraPosition = new Vector3(startRoomPostion.x, startRoomPostion.y, m_cameraMovement.transform.position.z);
            m_cameraMovement.gameObject.transform.position = newCameraPosition;
            m_cameraMovement.SetStartRoomTransform(m_path[0].transform);

            // init end room
            m_path[m_path.Count - 1].SetRoomType(RoomType.End);

            // replace blank rooms
            ReplaceAllRooms();

            // set up connections
            SetUpRoomConnections();

            // set up end room end zone
            m_path[m_path.Count - 1].EnableEndZone();

            // spawn player
            Vector3 spawnPosition = m_path[0].GetSpawnPosition().position;
            GameObject player = (GameObject)Instantiate(m_playerPref, spawnPosition, Quaternion.identity);
            Player playerClass = player.GetComponent<Player>();
            playerClass.ResetPlayer();
            m_cameraMovement.Init();

            // set up level data and pass to data tracker
            LevelData currentLevelData = new LevelData();
            currentLevelData.m_gridWidth = m_gridWidth;
            currentLevelData.m_gridHeight = m_gridHeight;
            currentLevelData.m_grid = m_rooms.ToArray();

            List<SimpleRoomData> roomData = new List<SimpleRoomData>();
            foreach (Room room in m_rooms)
            {
                roomData.Add(RoomParser.ParseRoomToString(room));
            }

            currentLevelData.m_simpleRoomData = roomData;

            RemoveEmptyRooms();

            DataTracker.Instance.AddNewLevelData(ref currentLevelData);
        }

        private void GeneratePath(out List<Room> path)
        {
            path = new List<Room>();

            // Generate start room
            Vector2Int startRoomPos = new Vector2Int(UnityEngine.Random.Range(0, m_gridWidth), 0);

            // decide whether to move left or right
            int xPos = startRoomPos.x;
            int yPos = 0;

            while (yPos < m_gridHeight)
            {
                Room room = GetRoom(xPos, yPos);
                room.SetRoomType(RoomType.Path); path.Add(room);

                int randLeftRight = UnityEngine.Random.Range(0, 100);
                if (randLeftRight < 50)
                {
                    Debug.Log("Moving Left");
                    // move left
                    // check if edge
                    if (xPos == 0)
                    {
                        Debug.Log("Left Edge");
                        yPos++;
                    }
                    else
                    {
                        int difference = xPos;
                        int moveAmount = UnityEngine.Random.Range(1, difference);
                        for (int moveIndex = (xPos - 1); moveIndex >= (xPos - moveAmount); moveIndex--)
                        {
                            if (moveIndex >= 0)
                            {
                                Room newRoom = GetRoom(moveIndex, yPos);
                                newRoom.SetRoomType(RoomType.Path);
                                path.Add(newRoom);
                            }
                        }
                        xPos -= moveAmount;
                        yPos++;
                    }
                }
                else
                {
                    Debug.Log("Moving Right");
                    // move right
                    // check if edge
                    if (xPos == (m_gridWidth - 1))
                    {
                        Debug.Log("RightEdge");
                        yPos++;
                    }
                    else
                    {
                        int difference = (m_gridWidth - 1) - xPos;
                        int moveAmount = UnityEngine.Random.Range(1, difference);
                        for (int moveIndex = (xPos + 1); moveIndex <= (xPos + moveAmount); moveIndex++)
                        {
                            if (moveIndex <= m_gridWidth - 1)
                            {
                                Room newRoom = GetRoom(moveIndex, yPos);
                                newRoom.SetRoomType(RoomType.Path);
                                path.Add(newRoom);
                            }
                        }
                        xPos += moveAmount;
                        yPos++;
                    }
                }
            }
        }

        private void ReplaceAllRooms()
        {
            for (int roomIndex = 0; roomIndex < m_path.Count; roomIndex++)
            {
                Room currentRoom = m_path[roomIndex];
                Vector3 roomPosition = currentRoom.gameObject.transform.position;
                GameObject newRoomObject;
                if (DataTracker.Instance.GetDifficulty() == Difficulty.Easy)
                {
                    newRoomObject = (GameObject)Instantiate(m_templateHolder.GetEasyGroup()
                            .m_roomPrefabs[UnityEngine.Random.Range(0,
                                m_templateHolder.GetEasyGroup().m_roomPrefabs.Length)],
                        roomPosition, Quaternion.identity);
                }
                else if (DataTracker.Instance.GetDifficulty() == Difficulty.Medium)
                {
                    newRoomObject = (GameObject)Instantiate(m_templateHolder.GetMediumGroup()
                            .m_roomPrefabs[UnityEngine.Random.Range(0,
                                m_templateHolder.GetMediumGroup().m_roomPrefabs.Length)],
                        roomPosition, Quaternion.identity);
                }
                else
                {
                    newRoomObject = (GameObject)Instantiate(m_templateHolder.GetHardGroup()
                            .m_roomPrefabs[UnityEngine.Random.Range(0,
                                m_templateHolder.GetHardGroup().m_roomPrefabs.Length)],
                        roomPosition, Quaternion.identity);
                }
                newRoomObject.transform.parent = this.transform;
                newRoomObject.name += "_" + currentRoom.GetPosition().x + "_" + currentRoom.GetPosition().y;
                Room replacementRoom = newRoomObject.GetComponent<Room>();
                replacementRoom.Init(currentRoom.GetRoomType(), currentRoom.GetPosition());
                m_path.RemoveAt(roomIndex);
                m_path.Insert(roomIndex, replacementRoom);
                int indexPosition = m_rooms.IndexOf(currentRoom);
                m_rooms.RemoveAt(indexPosition);
                m_rooms.Insert(indexPosition, replacementRoom);
                Destroy(currentRoom.gameObject);
            }
        }

        private void SetUpRoomConnections()
        {
            for (int roomIndex = 0; roomIndex < m_path.Count; roomIndex++)
            {
                // if start room only check next
                if (roomIndex == 0)
                {
                    Room currentRoom = m_path[roomIndex];
                    int nextIndex = roomIndex + 1;
                    Room nextRoom = m_path[nextIndex];
                    string connection;
                    CheckRoomConnections(nextRoom, currentRoom, out connection);
                    ConnectionType connectionType = (ConnectionType)Enum.Parse(typeof(ConnectionType), connection);
                    currentRoom.SetConnectionType(connectionType);
                    currentRoom.LinkRooms(connection, nextRoom);
                }
                else if (roomIndex == (m_path.Count - 1)) // if end room only check previous
                {
                    Room currentRoom = m_path[roomIndex];
                    int prevIndex = roomIndex - 1;
                    Room prevRoom = m_path[prevIndex];
                    string connection;
                    CheckRoomConnections(prevRoom, currentRoom, out connection);
                    ConnectionType connectionType = (ConnectionType)Enum.Parse(typeof(ConnectionType), connection);
                    currentRoom.SetConnectionType(connectionType);
                    currentRoom.LinkRooms(connection, prevRoom);
                }
                else //else check next and previous
                {
                    Room currentRoom = m_path[roomIndex];
                    int nextIndex = roomIndex + 1;
                    Room nextRoom = m_path[nextIndex];
                    string connectionOne;
                    CheckRoomConnections(nextRoom, currentRoom, out connectionOne);
                    currentRoom.LinkRooms(connectionOne, nextRoom);

                    int prevIndex = roomIndex - 1;
                    Room prevRoom = m_path[prevIndex];
                    string connectionTwo;
                    CheckRoomConnections(prevRoom, currentRoom, out connectionTwo);
                    currentRoom.LinkRooms(connectionTwo, prevRoom);

                    string connection = connectionOne + connectionTwo;
                    ConnectionType connectionType = (ConnectionType)Enum.Parse(typeof(ConnectionType), connection);
                    currentRoom.SetConnectionType(connectionType);
                }
            }
        }

        private void CheckRoomConnections(Room nextPrevRoom, Room currentRoom, out string connection)
        {
            // check for left connection
            if (nextPrevRoom.GetPosition().x < currentRoom.GetPosition().x)
            {
                connection = "L";
            }
            // check for right connection
            else if (nextPrevRoom.GetPosition().x > currentRoom.GetPosition().x)
            {
                connection = "R";
            }
            // check for up connection
            else if (nextPrevRoom.GetPosition().y > currentRoom.GetPosition().y)
            {
                connection = "U";
            }
            // check for down connection
            else
            {
                connection = "D";
            }
        }

        public Room GetRoom(int _x, int _y)
        {
            return m_rooms[(int)((_y * m_gridWidth) + _x)];
        }

        public void Clear()
        {
            foreach (Room room in m_rooms)
            {
                room.SetRoomType(RoomType.Empty);
                room.SetConnectionType(ConnectionType.none);
            }
        }

        private void RemoveEmptyRooms()
        {
            List<Room> emptyRooms = new List<Room>();
            foreach (Room room in m_rooms)
            {
                if (room.GetRoomType() == RoomType.Empty)
                {
                    Destroy(room.gameObject);
                }
            }
        }
    }
}