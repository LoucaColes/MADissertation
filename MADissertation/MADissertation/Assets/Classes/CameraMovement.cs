using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private Transform m_player;

    [SerializeField]
    private float m_horizontalOffset;

    [SerializeField]
    private float m_verticalOffset;

    private PlayerBounds m_bounds;
    private Vector3 m_lastPlayerPosition = Vector3.zero;

    // Use this for initialization
    private void Start()
    {
        m_bounds = CalculateNewPlayerBounds(this.transform.position);
    }

    // Update is called once per frame
    private void Update()
    {
        if (m_lastPlayerPosition != m_player.position)
        {
            // Check if player is within bounds
            Bounds bound;
            // If not in bounds move camera
            if (!CheckIfInBounds(out bound))
            {
                MoveCamera(bound);
            }

            // Update last position
            m_lastPlayerPosition = m_player.position;
        }
    }

    // Need to update to lerp
    private void MoveCamera(Bounds bound)
    {
        switch (bound)
        {
            case Bounds.Right:
                transform.position += new Vector3(m_horizontalOffset, 0f, 0f);
                break;

            case Bounds.Left:
                transform.position -= new Vector3(m_horizontalOffset, 0f, 0f);
                break;

            case Bounds.Top:
                transform.position += new Vector3(0f, m_verticalOffset, 0f);
                break;

            case Bounds.Botton:
                transform.position -= new Vector3(0f, m_verticalOffset, 0f);
                break;
        }
        m_bounds = CalculateNewPlayerBounds(transform.position);
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
}