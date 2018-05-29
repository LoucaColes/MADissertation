using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private Transform m_player;

    [SerializeField]
    private CameraType m_type;

    [SerializeField]
    private float m_horizontalOffset;

    [SerializeField]
    private float m_verticalOffset;

    [SerializeField]
    private float m_moveSpeed;

    [SerializeField]
    private LevelEdges m_levelEdge;

    private PlayerBounds m_bounds;
    private Vector3 m_lastPlayerPosition = Vector3.zero;
    private bool m_moving = false;
    private Vector3 m_nextPosition = Vector3.zero;
    private Rigidbody2D m_playerRigidbody;

    // Use this for initialization
    private void Start()
    {
        m_bounds = CalculateNewPlayerBounds(this.transform.position);
        m_playerRigidbody = m_player.gameObject.GetComponent<Rigidbody2D>();
        if (m_type == CameraType.EdgeLocking)
        {
            Vector3 playerBasedPostion = new Vector3(transform.position.x, m_player.position.y, transform.position.z);
            transform.position = playerBasedPostion;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_type == CameraType.PositionLocking)
        {
            Vector3 playerBasedPostion = new Vector3(m_player.position.x, m_player.position.y, transform.position.z);
            transform.position = playerBasedPostion;
        }
        else if (m_type == CameraType.EdgeLocking)
        {
            if (m_player.position.x > m_levelEdge.m_leftEdge && m_player.position.x < m_levelEdge.m_rightEdge
                && m_player.position.y > m_levelEdge.m_bottomEdge && m_player.position.y < m_levelEdge.m_topEdge)
            {
                Debug.Log("Moving");
                Vector3 playerBasedPostion = new Vector3(m_player.position.x, m_player.position.y, transform.position.z);
                transform.position = playerBasedPostion;
            }
            else
            {
                Debug.Log("Not Moving");
            }
        }
        //if (m_lastPlayerPosition != m_player.position)
        //{
        //    // Check if player is within bounds
        //    Bounds bound;
        //    // If not in bounds move camera
        //    if (!CheckIfInBounds(out bound))
        //    {
        //        Debug.Log("Out of bounds");
        //        MoveCamera(bound);
        //    }
        //    // Update last position
        //    m_lastPlayerPosition = m_player.position;
        //}
    }

    private void OnDrawGizmos()
    {
        Vector3 position = new Vector3(transform.position.x, transform.position.y, 0f);
        Vector3 cubeSize = new Vector3(2 * m_horizontalOffset, 2 * m_verticalOffset, 1f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(position, cubeSize);

        Vector3 nextposition = new Vector3(m_nextPosition.x, m_nextPosition.y, 0f);
        Vector3 nextCubeSize = new Vector3(2 * m_horizontalOffset, 2 * m_verticalOffset, 1f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(nextposition, nextCubeSize);
    }

    // Need to update to lerp
    private void MoveCamera(Bounds bound)
    {
        switch (bound)
        {
            case Bounds.Right:
                m_nextPosition = new Vector3(transform.position.x + m_horizontalOffset, transform.position.y, transform.position.z);
                break;

            case Bounds.Left:
                m_nextPosition = new Vector3(transform.position.x - m_horizontalOffset, transform.position.y, transform.position.z);
                break;

            case Bounds.Top:
                m_nextPosition = new Vector3(transform.position.x, transform.position.y + m_verticalOffset, transform.position.z);
                break;

            case Bounds.Botton:
                m_nextPosition = new Vector3(transform.position.x, transform.position.y - m_verticalOffset, transform.position.z);
                break;
        }
        transform.position = Vector3.MoveTowards(transform.position, m_nextPosition, m_moveSpeed * m_playerRigidbody.velocity.magnitude);
    }

    private PlayerBounds CalculateNewPlayerBounds(Vector3 _cameraPosition)
    {
        PlayerBounds newPlayerBounds = new PlayerBounds();
        newPlayerBounds.m_rightBound = _cameraPosition.x + m_horizontalOffset;
        newPlayerBounds.m_leftBound = _cameraPosition.x - m_horizontalOffset;
        newPlayerBounds.m_topBound = _cameraPosition.y + m_verticalOffset;
        newPlayerBounds.m_bottomBound = _cameraPosition.y - m_verticalOffset;
        return newPlayerBounds;
    }

    private bool CheckIfInBounds(out Bounds outwardBound)
    {
        bool inBounds = true;
        outwardBound = Bounds.Ignore;
        if (m_player.position.x > m_bounds.m_rightBound)
        {
            inBounds = false;
            outwardBound = Bounds.Right;
        }
        if (m_player.position.x < m_bounds.m_leftBound)
        {
            inBounds = false;
            outwardBound = Bounds.Left;
        }
        if (m_player.position.y > m_bounds.m_topBound)
        {
            inBounds = false;
            outwardBound = Bounds.Top;
        }
        if (m_player.position.y < m_bounds.m_bottomBound)
        {
            inBounds = false;
            outwardBound = Bounds.Botton;
        }
        return inBounds;
    }

    [System.Serializable]
    private struct PlayerBounds
    {
        public float m_rightBound;
        public float m_leftBound;
        public float m_topBound;
        public float m_bottomBound;
    }

    private enum Bounds
    {
        Right,
        Left,
        Top,
        Botton,
        Ignore
    }

    public enum CameraType
    {
        PositionLocking,
        EdgeLocking,
    }

    [System.Serializable]
    private struct LevelEdges
    {
        public float m_rightEdge;
        public float m_leftEdge;
        public float m_topEdge;
        public float m_bottomEdge;
    }
}