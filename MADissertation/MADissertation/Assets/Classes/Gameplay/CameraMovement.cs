using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Moves the camera based on the type of camera needed
/// </summary>
public class CameraMovement : MonoBehaviour
{
    // Debug options
    [SerializeField]
    private bool m_debugMode = true;

    // Designer variables
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
    private float m_transitionSpeed;

    [SerializeField]
    private LevelEdges m_levelEdge;

    [SerializeField]
    private Transform m_startRoomTransform;

    // private variables
    private CameraWindowBounds m_bounds;
    private Vector3 m_lastPlayerPosition = Vector3.zero;
    private bool m_moving = false;
    private Vector3 m_nextPosition = Vector3.zero;
    private Rigidbody2D m_playerRigidbody;
    private IEnumerator m_movementIenumerator = null;
    private Transform m_targetRoomTransform = null;
    private Transform m_currentRoomTransform = null;
    private Vector3 m_startPosition;
    private bool m_init = false;

    /// <summary>
    /// Handles set up
    /// </summary>
    public void Init()
    {
        m_bounds = CalculateNewCameraWindowBounds(this.transform.position);
        Player player = GameObject.FindObjectOfType<Player>();
        m_player = player.gameObject.transform;
        player.SetCameraMovement(this);
        m_playerRigidbody = m_player.gameObject.GetComponent<Rigidbody2D>();
        if (m_type == CameraType.EdgeLocking)
        {
            Vector3 playerBasedPostion = new Vector3(transform.position.x, m_player.position.y, transform.position.z);
            transform.position = playerBasedPostion;
        }
        m_currentRoomTransform = m_startRoomTransform;
        m_startPosition = transform.position;
        m_init = true;
    }

    /// <summary>
    /// Handles the updating of the camera position based on the players position
    /// </summary>
    private void Update()
    {
        if (m_init)
        {
            if (GameManager.Instance.GetGameState() == GameState.Play && m_player != null)
            {
                if (m_type == CameraType.PositionLocking)
                {
                    Vector3 playerBasedPostion = new Vector3(m_player.position.x, m_player.position.y,
                        transform.position.z);
                    transform.position = playerBasedPostion;
                }
                else if (m_type == CameraType.EdgeLocking)
                {
                    if (m_player.position.x > m_levelEdge.m_leftEdge && m_player.position.x < m_levelEdge.m_rightEdge
                        && m_player.position.y > m_levelEdge.m_bottomEdge &&
                        m_player.position.y < m_levelEdge.m_topEdge)
                    {
                        Vector3 playerBasedPostion =
                            new Vector3(m_player.position.x, m_player.position.y, transform.position.z);
                        transform.position = playerBasedPostion;
                    }
                }
                else if (m_type == CameraType.CameraWindowCentral)
                {
                    Bounds boundHit;
                    // Check if player is in camera window
                    if (CheckIfInBounds(out boundHit) && !m_moving) // If yes
                    {
                        if (m_movementIenumerator != null)
                        {
                            StopCoroutine(m_movementIenumerator);
                            m_movementIenumerator = null;
                            m_moving = false;
                        }
                    }
                    else // Else
                    {
                        // Calculate which bound was hit
                        // Calculate new camera window
                        CalculateNewCameraWindowPosition(boundHit);
                        // Move camera to centre of new window
                        m_movementIenumerator = MoveToNextPosition();
                        StartCoroutine(m_movementIenumerator);
                    }
                }
                else if (m_type == CameraType.RoomToRoom)
                {
                    if (m_targetRoomTransform != null && (m_targetRoomTransform != m_currentRoomTransform))
                    {
                        Vector3 targetPosition = new Vector3(m_targetRoomTransform.position.x,
                            m_targetRoomTransform.position.y, transform.position.z);
                        transform.position =
                            Vector3.MoveTowards(transform.position, targetPosition, m_transitionSpeed * Time.deltaTime);
                        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                        {
                            transform.position = targetPosition;
                            m_currentRoomTransform = m_targetRoomTransform;
                            m_targetRoomTransform = null;
                        }
                    }
                }
            }
        }
    }

    public void SetNewRoomTarget(Transform _targetTransform)
    {
        m_targetRoomTransform = _targetTransform;
    }

    public Transform HasRoomTarget()
    {
        return m_targetRoomTransform;
    }

    public Transform CurrentTargetTransform()
    {
        return m_currentRoomTransform;
    }

    public void SetStartRoomTransform(Transform _startRoomTransform)
    {
        m_startRoomTransform = _startRoomTransform;
    }

    public void ResetCamera()
    {
        StopAllCoroutines();
        m_moving = false;
        m_targetRoomTransform = null;
        transform.position = m_startPosition;
    }

    /// <summary>
    /// Handles drawing of bounds for debugging purposes
    /// </summary>
    private void OnDrawGizmos()
    {
        if (m_debugMode)
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
    }

    /// <summary>
    /// Update camera position based on bound that was hit
    /// </summary>
    /// <param name="bound">Bound that the player hit</param>
    private void CalculateNewCameraWindowPosition(Bounds bound)
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
    }

    private IEnumerator MoveToNextPosition()
    {
        while (Vector3.Distance(transform.position, m_nextPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_nextPosition, m_moveSpeed * m_playerRigidbody.velocity.magnitude);
            m_moving = true;
            m_bounds = CalculateNewCameraWindowBounds(transform.position);
            yield return null;
        }
        if (Vector3.Distance(transform.position, m_nextPosition) <= 0.1f)
        {
            transform.position = m_nextPosition;
            m_bounds = CalculateNewCameraWindowBounds(transform.position);
            m_moving = false;
            if (m_movementIenumerator != null)
            {
                StopCoroutine(m_movementIenumerator);
                m_movementIenumerator = null;
            }
        }
    }

    /// <summary>
    /// Calculates the new camera window bounds
    /// </summary>
    /// <param name="_cameraPosition">Position of the camera</param>
    /// <returns>Returns new camera window bounds</returns>
    private CameraWindowBounds CalculateNewCameraWindowBounds(Vector3 _cameraPosition)
    {
        CameraWindowBounds newCameraWindowBounds = new CameraWindowBounds();
        newCameraWindowBounds.m_rightBound = _cameraPosition.x + m_horizontalOffset;
        newCameraWindowBounds.m_leftBound = _cameraPosition.x - m_horizontalOffset;
        newCameraWindowBounds.m_topBound = _cameraPosition.y + m_verticalOffset;
        newCameraWindowBounds.m_bottomBound = _cameraPosition.y - m_verticalOffset;
        return newCameraWindowBounds;
    }

    /// <summary>
    /// Checks if the player is within the camera window bounds
    /// </summary>
    /// <param name="outwardBound">Returns which bound was hit</param>
    /// <returns>Returns true if the player is with the camera window bounds</returns>
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

    /// <summary>
    /// Used to define the camera window bounds
    /// </summary>
    [System.Serializable]
    private struct CameraWindowBounds
    {
        public float m_rightBound;
        public float m_leftBound;
        public float m_topBound;
        public float m_bottomBound;
    }

    /// <summary>
    /// Used to define which bound was reached by the player
    /// </summary>
    private enum Bounds
    {
        Right,
        Left,
        Top,
        Botton,
        Ignore
    }

    /// <summary>
    /// Type of camera that will be used
    /// </summary>
    public enum CameraType
    {
        PositionLocking,
        EdgeLocking,
        CameraWindowCentral,
        RoomToRoom,
    }

    /// <summary>
    /// Used to define the edges of the level
    /// </summary>
    [System.Serializable]
    private struct LevelEdges
    {
        public float m_rightEdge;
        public float m_leftEdge;
        public float m_topEdge;
        public float m_bottomEdge;
    }
}