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
    private Transform m_startRoomTransform;

    // private variables
    private Player m_player;
    private Vector3 m_lastPlayerPosition = Vector3.zero;
    private Vector3 m_nextPosition = Vector3.zero;
    private Transform m_targetRoomTransform = null;
    private Transform m_currentRoomTransform = null;
    private Vector3 m_startPosition;
    private bool m_init = false;

    /// <summary>
    /// Handles set up
    /// </summary>
    public void Init()
    {
        // Find the player object
        m_player = GameObject.FindObjectOfType<Player>();

        // Give the player a reference to this camera movement class
        m_player.SetCameraMovement(this);

        // Set the current room transform
        m_currentRoomTransform = m_startRoomTransform;

        // Set the start position
        m_startPosition = transform.position;

        // Set init to true
        m_init = true;
    }

    /// <summary>
    /// Handles the updating of the camera position based on the players position
    /// </summary>
    private void Update()
    {
        // If the camera has initialised
        if (m_init)
        {
            // If the game state is play and we have a player
            if (GameManager.Instance.GetGameState() == GameState.Play && m_player != null)
            {
                // Check if we dont have a target room transform and the target room transform doesn't equal
                // the current room transform
                if (m_targetRoomTransform != null && (m_targetRoomTransform != m_currentRoomTransform))
                {
                    // Calculate the target position
                    Vector3 targetPosition = new Vector3(m_targetRoomTransform.position.x,
                        m_targetRoomTransform.position.y, transform.position.z);

                    // Use move towards to set the cameras position over time
                    transform.position =
                        Vector3.MoveTowards(transform.position, targetPosition, m_transitionSpeed * Time.deltaTime);

                    // Check if we are close to the target position
                    if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
                    {
                        // Set the camera position to the target position
                        transform.position = targetPosition;

                        // Set the current room transform to the target room transform
                        m_currentRoomTransform = m_targetRoomTransform;

                        // Set the target room transform to null
                        m_targetRoomTransform = null;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Set the target room transform
    /// </summary>
    /// <param name="_targetTransform">New target transform</param>
    public void SetNewRoomTarget(Transform _targetTransform)
    {
        m_targetRoomTransform = _targetTransform;
    }

    /// <summary>
    /// Get the target room transform
    /// </summary>
    /// <returns>Target room transform</returns>
    public Transform HasRoomTarget()
    {
        return m_targetRoomTransform;
    }

    /// <summary>
    /// Get the current room trasnform
    /// </summary>
    /// <returns>Current room transform</returns>
    public Transform CurrentTargetTransform()
    {
        return m_currentRoomTransform;
    }

    /// <summary>
    /// Set the start room transform
    /// </summary>
    /// <param name="_startRoomTransform">Start room transform</param>
    public void SetStartRoomTransform(Transform _startRoomTransform)
    {
        m_startRoomTransform = _startRoomTransform;
    }

    /// <summary>
    /// Reset the camera
    /// </summary>
    public void ResetCamera()
    {
        // Set the target room to null
        m_targetRoomTransform = null;

        // Set the camera position to the start position
        transform.position = m_startPosition;
    }

    /// <summary>
    /// Type of camera that will be used
    /// </summary>
    public enum CameraType
    {
        RoomToRoom,
    }
}