using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

/// <summary>
/// Human controlled player class
/// </summary>
public class Player : Character
{
    // Designer Variables
    [SerializeField]
    private int m_playerId = 0;

    [SerializeField]
    private float m_sprintMultiplier = 2f;

    [SerializeField]
    private float m_heightOffset = 0.1f;

    [SerializeField]
    private float m_dropOffset;

    [SerializeField]
    private float m_raycastLength = 0.1f;

    [SerializeField]
    private float m_dashCooldown = 0.5f;

    [SerializeField]
    private float m_dashForce = 100f;

    // Private variables
    private Rewired.Player m_rewiredPlayer;
    private Vector3 m_movementInput;
    private bool m_sprinting = false;
    private float m_movementMultiplier = 1;
    private Collider2D m_collider;
    private float m_height;
    private bool m_grounded = false;
    private int m_score;
    private Vector2 m_pausedVelocity;
    private CameraMovement m_cameraMovement;
    private Vector2 m_startPosition;
    private float m_dashTimer = 0;

    /// <summary>
    /// Handle Set Up
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        m_rewiredPlayer = ReInput.players.GetPlayer(m_playerId);
        m_collider = GetComponent<Collider2D>();
        m_height = m_collider.bounds.extents.y + m_heightOffset;
        m_startPosition = transform.position;
    }

    public void SetCameraMovement(CameraMovement _cameraMovement)
    {
        m_cameraMovement = _cameraMovement;
    }

    /// <summary>
    /// Handle Input
    /// </summary>
    // Update is called once per frame
    protected override void Update()
    {
        if (GameManager.Instance.GetGameState() == GameState.Play)
        {
            // Check if grounded
            Vector2 startPosition = new Vector2(transform.position.x, transform.position.y - m_height);
            m_grounded = Physics2D.Raycast(startPosition, Vector2.down, m_raycastLength);

            // Handle Jump Input
            if (m_rewiredPlayer.GetButtonDown("Jump"))
            {
                Jump();
            }

            // Handle Sprint Input
            if (m_rewiredPlayer.GetButtonDown("Sprint"))
            {
                m_sprinting = true;
                m_movementMultiplier = m_sprintMultiplier;
            }
            else if (m_rewiredPlayer.GetButtonUp("Sprint"))
            {
                m_sprinting = false;
                m_movementMultiplier = 1;
            }

            // Handle Movement Input
            m_movementInput = new Vector3(m_rewiredPlayer.GetAxis("MoveHorizontal"), 0f, 0f);

            // Handle Pause
            if (m_rewiredPlayer.GetButtonDown("Pause"))
            {
                GameManager.Instance.ChangeState(GameState.Pause);
            }

            if (m_rewiredPlayer.GetAxis("MoveVertical") < 0 && m_rewiredPlayer.GetButtonDown("Drop"))
            {
                float height = m_collider.bounds.extents.y + m_dropOffset;
                Vector2 checkPosition = new Vector2(transform.position.x, transform.position.y - height);
                RaycastHit2D hit2D = Physics2D.Raycast(checkPosition, Vector2.down, m_raycastLength);
                Debug.DrawRay(checkPosition, Vector2.down * m_raycastLength, Color.cyan);
                if (hit2D.transform.gameObject.tag == "OneWayPlatform")
                {
                    hit2D.collider.enabled = false;
                }
            }

            m_dashTimer += Time.deltaTime;
            if (m_rewiredPlayer.GetButtonDown("DashRight") && m_dashTimer > m_dashCooldown)
            {
                m_rigidBody.AddForce(Vector2.right * m_dashForce * Time.deltaTime);
                m_dashTimer = 0;
            }

            if (m_rewiredPlayer.GetButtonDown("DashLeft") && m_dashTimer > m_dashCooldown)
            {
                m_rigidBody.AddForce(Vector2.left * m_dashForce * Time.deltaTime);
                m_dashTimer = 0;
            }

            base.Update();
        }
    }

    /// <summary>
    /// Handle physics
    /// </summary>
    protected override void FixedUpdate()
    {
        if (GameManager.Instance.GetGameState() == GameState.Play)
        {
            // Calculate movement based on whether the player is grounded or not
            Vector2 movement;
            if (!m_grounded)
            {
                movement = new Vector2(m_movementInput.x * m_characterData.m_moveSpeed * m_movementMultiplier,
                    m_gravity);
            }
            else
            {
                movement = new Vector2(m_movementInput.x * m_characterData.m_moveSpeed * m_movementMultiplier, 0);
            }

            // Update movement
            m_rigidBody.AddForce(movement * Time.deltaTime);
        }
    }

    /// <summary>
    /// Makes the player jump in the air
    /// </summary>
    private void Jump()
    {
        // If player is grounded then jump
        if (m_grounded)
        {
            Vector2 movement = new Vector2(m_movementInput.x * m_characterData.m_moveSpeed * m_movementMultiplier,
                m_characterData.m_jumpSpeed);
            m_rigidBody.AddForce(movement * Time.deltaTime);
        }
    }

    /// <summary>
    /// Handles collisions with collectables and end zones
    /// </summary>
    /// <param name="other"> The other object being collided with</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        // If end zone then load next level
        if (other.tag == "EndZone")
        {
            GameManager.Instance.ChangeState(GameState.LevelOver);
        }
        // If collectable then increase the score and destroy collectable
        if (other.tag == "Collectable")
        {
            Collectable collectable = other.gameObject.GetComponent<Collectable>();
            m_score += collectable.Collect();
            GameManager.Instance.UpdateCurrentLevelScore(m_score);
            Destroy(other.gameObject);
        }

        if (other.tag == "Entrance")
        {
            if (m_cameraMovement)
            {
                RoomExit exit = other.gameObject.GetComponent<RoomExit>();
                if (exit.GetRoom() != null)
                {
                    Transform targetTransform = exit.GetRoom().transform;
                    if (targetTransform != m_cameraMovement.CurrentTargetTransform())
                    {
                        m_cameraMovement.SetNewRoomTarget(targetTransform);
                    }
                }
            }
        }
        if (other.tag == "Obstacle")
        {
            m_currentHealth--;
            GameManager.Instance.UpdateCurrentLevelLives(m_currentHealth);
            GameManager.Instance.ChangeState(GameState.GameOver);
            Vector2 velocity = m_rigidBody.velocity;
            m_rigidBody.velocity += -velocity;
            m_renderer.enabled = false;
        }
        if (other.tag == "OneWayTrigger")
        {
            OneWayPlatform oneWayPlatform = other.gameObject.GetComponent<OneWayPlatform>();
            oneWayPlatform.DisableSolidCollider();
            oneWayPlatform.StartCoroutine(oneWayPlatform.EnableAfter());
        }
    }

    public void ResetPlayer()
    {
        transform.position = m_startPosition;
        Vector2 velocity = m_rigidBody.velocity;
        m_rigidBody.velocity += -velocity;
        m_renderer.enabled = true;
    }

    public void Freeze()
    {
        m_pausedVelocity = m_rigidBody.velocity;
        m_rigidBody.velocity += -m_pausedVelocity;
        m_rigidBody.gravityScale = 0;
    }

    public void UnFreeze()
    {
        m_rigidBody.velocity += m_pausedVelocity;
        m_rigidBody.gravityScale = 1;
    }
}