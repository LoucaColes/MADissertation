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

    [SerializeField]
    private float m_dashTime = 0.25f;

    [SerializeField]
    private float m_jumpTime = 0.35f;

    [SerializeField]
    private Transform m_rightWallCheck;

    [SerializeField]
    private Transform m_leftWallCheck;

    [SerializeField]
    private float m_wallCheckRadius = 0.3f;

    [SerializeField]
    private LayerMask m_wallMask;

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
    private bool m_dashing = false;
    private float m_dashingTimer = 0;
    private float m_jumpTimer = 0;
    private bool m_jumping = false;

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
        m_jumpTimer = m_jumpTime;
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

            if (m_rewiredPlayer.GetButton("Jump") && m_jumping)
            {
                if (m_jumpTimer > 0)
                {
                    m_rigidBody.velocity = Vector2.up * m_characterData.m_jumpSpeed * m_movementMultiplier;
                    m_jumpTimer -= Time.deltaTime;
                }
                else
                {
                    m_jumping = false;
                }
            }

            if (m_rewiredPlayer.GetButtonUp("Jump"))
            {
                m_jumping = false;
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
                float xVelocity = m_rigidBody.velocity.x;
                float yVelocity = m_rigidBody.velocity.y;
                Vector2 invertedVelocity = new Vector2(xVelocity, -yVelocity);
                m_rigidBody.velocity += invertedVelocity;
                m_rigidBody.AddForce(Vector2.right * m_dashForce * Time.deltaTime);
                m_dashTimer = 0;
                m_dashing = true;
            }

            if (m_rewiredPlayer.GetButtonDown("DashLeft") && m_dashTimer > m_dashCooldown)
            {
                float xVelocity = m_rigidBody.velocity.x;
                float yVelocity = m_rigidBody.velocity.y;
                Vector2 invertedVelocity = new Vector2(xVelocity, -yVelocity);
                m_rigidBody.velocity += invertedVelocity;
                m_rigidBody.AddForce(Vector2.left * m_dashForce * Time.deltaTime);
                m_dashTimer = 0;
                m_dashing = true;
            }

            if (m_dashing)
            {
                m_dashingTimer += Time.deltaTime;
                if (m_dashingTimer >= m_dashTime)
                {
                    m_dashingTimer = 0;
                    m_dashing = false;
                }
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
            movement = new Vector2(m_movementInput.x * m_characterData.m_moveSpeed * m_movementMultiplier, m_rigidBody.velocity.y);

            // Update movement
            m_rigidBody.velocity = movement;
        }
    }

    /// <summary>
    /// Makes the player jump in the air
    /// </summary>
    private void Jump()
    {
        bool wallCheckLeft = Physics2D.OverlapCircle(m_leftWallCheck.position, m_wallCheckRadius, m_wallMask);
        Debug.Log("Wall Check Left: " + wallCheckLeft);
        bool wallCheckRight = Physics2D.OverlapCircle(m_rightWallCheck.position, m_wallCheckRadius, m_wallMask);
        Debug.Log("Wall Check Right: " + wallCheckRight);
        // If player is grounded then jump
        if (m_grounded || wallCheckRight || wallCheckLeft)
        {
            m_jumping = true;
            m_jumpTimer = m_jumpTime;
            m_rigidBody.velocity = Vector2.up * m_characterData.m_jumpSpeed * m_movementMultiplier;
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