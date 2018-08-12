using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelGeneration
{
    /// <summary>
    /// Room class that controls customisation and linking between other rooms
    /// </summary>
    public class Room : MonoBehaviour
    {
        // Designer Variables
        // Debug renderer to visually check if what type of room it is
        [SerializeField]
        private SpriteRenderer m_testRenderer;

        // Blocks that allow/stop the player from going upwards
        [SerializeField]
        private GameObject[] m_topAccessBlocks;

        // Blocks that allow/stop the player from going downwards
        [SerializeField]
        private GameObject[] m_bottomAccessBlocks;

        // Blocks that allow/stop the player from going left
        [SerializeField]
        private GameObject[] m_leftAccessBlocks;

        // Blocks that allow/stop the player from going right
        [SerializeField]
        private GameObject[] m_rightAccessBlocks;

        // The upwards room exit trigger
        [SerializeField]
        private GameObject m_topExit;

        // The downwards room exit trigger
        [SerializeField]
        private GameObject m_bottomExit;

        // The left room exit trigger
        [SerializeField]
        private GameObject m_leftExit;

        // The right room exit trigger
        [SerializeField]
        private GameObject m_rightExit;

        // The upwards room exit trigger class
        [SerializeField]
        private RoomExit m_topExitClass;

        // The downwards room exit trigger class
        [SerializeField]
        private RoomExit m_bottomExitClass;

        // The left room exit trigger class
        [SerializeField]
        private RoomExit m_leftExitClass;

        // The right room exit trigger class
        [SerializeField]
        private RoomExit m_rightExitClass;

        // End zone game object
        [SerializeField]
        private GameObject m_endZoneObject;

        // Spawn position for the player
        [SerializeField]
        private Transform m_spawnPoint;

        [SerializeField]
        private GameObject m_collectable;

        private Collectable m_collectableClass;

        // Type of room
        private RoomType m_type;

        // Type of connection the room has (see ConnectionType for more)
        private ConnectionType m_connectionType;

        // Grid position of the room (not world space)
        private Vector2Int m_position;

        /// <summary>
        /// Initialise the room with room type and grid position
        /// </summary>
        /// <param name="_type"></param>
        /// <param name="_position"></param>
        public void Init(RoomType _type, Vector2Int _position)
        {
            // Set the room type
            m_type = _type;

            // If there is a debug renderer update the colour based on room type
            if (m_testRenderer != null)
            {
                // Set render colour based on the room type
                if (m_type == RoomType.Path)
                {
                    m_testRenderer.color = Color.cyan;
                }
                else if (m_type == RoomType.Start)
                {
                    m_testRenderer.color = Color.red;
                }
                else if (m_type == RoomType.End)
                {
                    m_testRenderer.color = Color.magenta;
                }

                // If not in debug mode disable the debug renderer
                if (!GameManager.Instance.IsDebug())
                {
                    m_testRenderer.gameObject.SetActive(false);
                }
            }
            // Set the grid position
            m_position = _position;
        }

        /// <summary>
        /// Set the room type
        /// </summary>
        /// <param name="_type"></param>
        public void SetRoomType(RoomType _type)
        {
            // Set the room type
            m_type = _type;

            // If there is a debug renderer update the colour based on room type
            if (m_testRenderer != null)
            {
                // Set render colour based on room type
                if (m_type == RoomType.Path)
                {
                    m_testRenderer.color = Color.cyan;
                }
                else if (m_type == RoomType.Start)
                {
                    m_testRenderer.color = Color.red;
                }
                else if (m_type == RoomType.End)
                {
                    m_testRenderer.color = Color.magenta;
                }
                else
                {
                    m_testRenderer.color = Color.white;
                }

                // If not in debug mode disable the debug renderer
                if (!GameManager.Instance.IsDebug())
                {
                    m_testRenderer.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Set the connection type
        /// </summary>
        /// <param name="_type"></param>
        public void SetConnectionType(ConnectionType _type)
        {
            // Set the connection type
            m_connectionType = _type;

            // Convert the connections to a string
            string connection = m_connectionType.ToString();

            // Check if there is a left connection
            if (connection.Contains("L"))
            {
                // Loop through all of the left access blocks and set the to not active
                for (int blockIndex = 0; blockIndex < m_leftAccessBlocks.Length; blockIndex++)
                {
                    m_leftAccessBlocks[blockIndex].SetActive(false);
                }

                // Set the left exit to active
                m_leftExit.SetActive(true);
            }

            // Check if there is a right connection
            if (connection.Contains("R"))
            {
                // Loop through all of the right access blocks and set the to not active
                for (int blockIndex = 0; blockIndex < m_rightAccessBlocks.Length; blockIndex++)
                {
                    m_rightAccessBlocks[blockIndex].SetActive(false);
                }

                // Set the right exit to active
                m_rightExit.SetActive(true);
            }

            // Check if there is an up connection
            if (connection.Contains("U"))
            {
                // Loop through all of the up access blocks and set the to not active
                for (int blockIndex = 0; blockIndex < m_topAccessBlocks.Length; blockIndex++)
                {
                    m_topAccessBlocks[blockIndex].SetActive(false);
                }

                // Set the top exit to active
                m_topExit.SetActive(true);
            }

            // Check if there is a down connection
            if (connection.Contains("D"))
            {
                // Loop through all of the left access blocks and set the to not active
                for (int blockIndex = 0; blockIndex < m_bottomAccessBlocks.Length; blockIndex++)
                {
                    m_bottomAccessBlocks[blockIndex].SetActive(false);
                }

                // Set the bottom exit to active
                m_bottomExit.SetActive(true);
            }
        }

        /// <summary>
        /// Get the room type
        /// </summary>
        /// <returns>Room type</returns>
        public RoomType GetRoomType()
        {
            return m_type;
        }

        /// <summary>
        /// Get the room position
        /// </summary>
        /// <returns>Room position</returns>
        public Vector2Int GetPosition()
        {
            return m_position;
        }

        /// <summary>
        /// Link the two rooms together
        /// </summary>
        /// <param name="_connection">Connection string</param>
        /// <param name="_connectedRoom">Connected Room</param>
        public void LinkRooms(string _connection, Room _connectedRoom)
        {
            // Check if there is a left connection
            if (_connection.Contains("L"))
            {
                // Pass the connected room ref to the left exit class
                m_leftExitClass.SetConnectedRoom(_connectedRoom);
            }

            // Check if there is a right connection
            if (_connection.Contains("R"))
            {
                // Pass the connected room ref to the right exit class
                m_rightExitClass.SetConnectedRoom(_connectedRoom);
            }

            // Check if there is a up connection
            if (_connection.Contains("U"))
            {
                // Pass the connected room ref to the top exit class
                m_topExitClass.SetConnectedRoom(_connectedRoom);
            }

            // Check if there is a down connection
            if (_connection.Contains("D"))
            {
                // Pass the connected room ref to the bottom exit class
                m_bottomExitClass.SetConnectedRoom(_connectedRoom);
            }
        }

        /// <summary>
        /// Attempt to spawn a collectable in the room
        /// </summary>
        /// <param name="_currentCount">Current collectable count</param>
        /// <param name="_collectableCountMax">Max amount of collectables that can spawn</param>
        public void AttemptSpawnCollectable(ref int _currentCount, int _collectableCountMax)
        {
            // Set collectable to active
            m_collectable.SetActive(true);

            // Get the collectable class
            m_collectableClass = m_collectable.GetComponent<Collectable>();

            // Get the spawnrate
            int spawnRate = m_collectableClass.GetSpawnRate();

            // Randomly decide if the collectable should spawn
            if (_currentCount < _collectableCountMax && Random.Range(0, spawnRate) == 1)
            {
                // If spawned increase the collectable count
                _currentCount++;
            }
            else
            {
                // If not spawned set active to false
                m_collectable.SetActive(false);
            }
        }

        /// <summary>
        /// Enable the end zone game object
        /// </summary>
        public void EnableEndZone()
        {
            m_endZoneObject.SetActive(true);
        }

        /// <summary>
        /// Get the player spawn position
        /// </summary>
        /// <returns>Player spawn position</returns>
        public Transform GetSpawnPosition()
        {
            return m_spawnPoint;
        }
    }

    /// <summary>
    /// Room type enum
    /// </summary>
    public enum RoomType
    {
        Start,
        End,
        Path,
        Empty
    }

    /// <summary>
    /// Connections that the room has
    /// L - Left
    /// R - Right
    /// U - Up
    /// D - Down
    /// </summary>
    public enum ConnectionType
    {
        L,
        R,
        U,
        D,
        LR,
        LU,
        LD,
        RL,
        RU,
        RD,
        UL,
        UR,
        UD,
        DL,
        DR,
        DU,
        none,
        //LRD,
        //LRU,
        //LDU,
        //RDU,
        //All
    }
}