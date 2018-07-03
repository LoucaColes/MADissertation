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

        // Top cover that stops the player falling back down
        [SerializeField]
        private GameObject m_topCover;

        // Bottom cover that stops the player falling down
        [SerializeField]
        private GameObject m_bottomCover;

        // End zone game object
        [SerializeField]
        private GameObject m_endZoneObject;

        // Spawn position for the player
        [SerializeField]
        private Transform m_spawnPoint;

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
            m_type = _type;
            // If there is a debug renderer update the colour based on room type
            if (m_testRenderer != null)
            {
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
            m_connectionType = _type;
            // convert the connections to a string
            string connection = m_connectionType.ToString();
            // check if there is a left connection
            if (connection.Contains("L"))
            {
                for (int blockIndex = 0; blockIndex < m_leftAccessBlocks.Length; blockIndex++)
                {
                    m_leftAccessBlocks[blockIndex].SetActive(false);
                }
                m_leftExit.SetActive(true);
            }

            if (connection.Contains("R"))
            {
                for (int blockIndex = 0; blockIndex < m_rightAccessBlocks.Length; blockIndex++)
                {
                    m_rightAccessBlocks[blockIndex].SetActive(false);
                }
                m_rightExit.SetActive(true);
            }

            if (connection.Contains("U"))
            {
                for (int blockIndex = 0; blockIndex < m_topAccessBlocks.Length; blockIndex++)
                {
                    m_topAccessBlocks[blockIndex].SetActive(false);
                }
                m_topExit.SetActive(true);
            }

            if (connection.Contains("D"))
            {
                for (int blockIndex = 0; blockIndex < m_bottomAccessBlocks.Length; blockIndex++)
                {
                    m_bottomAccessBlocks[blockIndex].SetActive(false);
                }
                m_bottomExit.SetActive(true);
            }
        }

        public RoomType GetRoomType()
        {
            return m_type;
        }

        public Vector2Int GetPosition()
        {
            return m_position;
        }

        public void LinkRooms(string _connection, Room _connectedRoom)
        {
            m_topCover.SetActive(false);
            m_bottomCover.SetActive(false);
            if (_connection.Contains("L"))
            {
                m_leftExitClass.SetConnectedRoom(_connectedRoom);
            }

            if (_connection.Contains("R"))
            {
                m_rightExitClass.SetConnectedRoom(_connectedRoom);
            }

            if (_connection.Contains("U"))
            {
                m_topExitClass.SetConnectedRoom(_connectedRoom);
                m_topCover.SetActive(true);
            }
            //else
            //{
            //    m_topCover.SetActive(false);
            //}

            if (_connection.Contains("D"))
            {
                m_bottomExitClass.SetConnectedRoom(_connectedRoom);
                m_bottomCover.SetActive(true);
            }
            //else
            //{
            //    m_bottomCover.SetActive(false);
            //}
        }

        public void EnableEndZone()
        {
            m_endZoneObject.SetActive(true);
        }

        public Transform GetSpawnPosition()
        {
            return m_spawnPoint;
        }
    }

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