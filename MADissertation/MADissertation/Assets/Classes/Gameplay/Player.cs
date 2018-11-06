using LevelGeneration;
using Rewired;
using UnityEngine;

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
    private Transform m_groundCheck;

    [SerializeField]
    private Transform m_rightCoyoteCheck;

    [SerializeField]
    private Transform m_leftCoyoteCheck;

    [SerializeField]
    private float m_wallCheckRadius = 0.3f;

    [SerializeField]
    private float m_groundCheckRadius = 0.3f;

    [SerializeField]
    private float m_coyoteCheckRadius = 0.3f;

    [SerializeField]
    private LayerMask m_wallMask;

    [SerializeField]
    private GameObject m_jumpPSPrefab;

    [SerializeField]
    private float m_slideFactor = 1.1f;

    [SerializeField]
    private Sprite m_sprintRightSprite;

    [SerializeField]
    private Sprite m_sprintLeftSprite;

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
    private Vector2 m_startPosition;
    private float m_dashTimer = 0;
    private bool m_dashing = false;
    private bool m_dashingLeft = false;
    private bool m_dashingRight = false;
    private float m_dashingTimer = 0;
    private float m_jumpTimer = 0;
    private bool m_jumping = false;
    private Room m_currentRoom;
    private bool m_heldJumping = false;
    private bool m_wallJumpRight = false;
    private bool m_wallJumpLeft = false;

    /// <summary>
    /// Handle Set Up
    /// </summary>
    protected override void Awake()
    {
        // Use the base class awake
        base.Awake();

        // Get the rewired player
        m_rewiredPlayer = ReInput.players.GetPlayer(m_playerId);

        // Get the collider
        m_collider = GetComponent<Collider2D>();

        // Calculate the height of the collider bounds
        m_height = m_collider.bounds.extents.y + m_heightOffset;

        // Set the jump timer equal to jump time
        m_jumpTimer = m_jumpTime;
    }

    /// <summary>
    /// Handle Input
    /// </summary>
    // Update is called once per frame
    protected override void Update()
    {
        // If game state is equal to play
        if (GameManager.Instance.GetGameState() == GameState.Play)
        {
            // Handle Jump Input
            // If the player taps the jump button
            if (m_rewiredPlayer.GetButtonDown("Jump"))
            {
                Jump();
            }

            // If the player is holding down the jump button and jumping equals true
            if (m_rewiredPlayer.GetButton("Jump") && m_jumping)
            {
                m_heldJumping = true;
            }

            // If the player releases the jump button set jumping to false
            if (m_rewiredPlayer.GetButtonUp("Jump"))
            {
                m_jumping = false;
            }

            // Handle Sprint Input
            // If the player presses the sprint button
            if (m_rewiredPlayer.GetButtonDown("Sprint"))
            {
                // Set sprint to true
                m_sprinting = true;

                // Set the movement multiplier equal to the sprint multiplier
                m_movementMultiplier = m_sprintMultiplier;
            }

            // If the player releases the sprint button
            else if (m_rewiredPlayer.GetButtonUp("Sprint"))
            {
                // Set sprint to false
                m_sprinting = false;

                // Set the movement multiplier to 1
                m_movementMultiplier = 1;
            }

            // Handle Movement Input
            m_movementInput = new Vector3(m_rewiredPlayer.GetAxis("MoveHorizontal"), 0f, 0f);

            // Handle Pause
            // If the pause button is pressed set the gamestate to pause
            if (m_rewiredPlayer.GetButtonDown("Pause"))
            {
                GameManager.Instance.ChangeState(GameState.Pause);
            }

            // Handle dashing
            // If the dash right button is pressed
            if (m_rewiredPlayer.GetButtonDown("DashRight"))
            {
                // Set dashing and dashing right to true
                m_dashing = true;
                m_dashingRight = true;

                // Set dashing left to false
                m_dashingLeft = false;
            }

            // If the dash left button is pressed
            if (m_rewiredPlayer.GetButtonDown("DashLeft"))
            {
                // Set dashing and dashing left to true
                m_dashing = true;
                m_dashingLeft = true;

                // Set dashing right to false
                m_dashingRight = false;
            }

            // Use base class update
            base.Update();
        }
    }

    /// <summary>
    /// Handle physics
    /// </summary>
    protected override void FixedUpdate()
    {
        // If game state is play
        if (GameManager.Instance.GetGameState() == GameState.Play)
        {
            // Calculate movement based on whether the player is grounded or not
            Vector2 movement;
            if (!m_dashing)
            {
                // Check if there is a wall to the left
                bool wallCheckLeft = Physics2D.OverlapCircle(m_leftWallCheck.position, m_wallCheckRadius, m_wallMask);
                // Check if there is a wall to the right
                bool wallCheckRight = Physics2D.OverlapCircle(m_rightWallCheck.position, m_wallCheckRadius, m_wallMask);

                // If there is a wall to the right of the player and the player is moving left
                if (wallCheckRight && m_movementInput.x < 0)
                {
                    // Use normal movement calculation
                    movement = new Vector2(
                        m_movementInput.x * m_characterData.m_moveSpeed * m_movementMultiplier * Time.deltaTime,
                        m_rigidBody.velocity.y);
                }
                // If there is a wall to the left of the player and the player is moving right
                else if (wallCheckLeft && m_movementInput.x > 0)
                {
                    // Use normal movement calculation
                    movement = new Vector2(
                        m_movementInput.x * m_characterData.m_moveSpeed * m_movementMultiplier * Time.deltaTime,
                        m_rigidBody.velocity.y);
                }
                // If there is a wall to the left and the player is moving to the left
                // Or there is a wall to the right and the player is moving to the right
                else if ((wallCheckLeft && m_movementInput.x <= 0) || (wallCheckRight && m_movementInput.x >= 0))
                {
                    // Slide down the wall and ignore any horizontal input
                    movement = new Vector2(
                        0f,
                        m_rigidBody.velocity.y / m_slideFactor);
                }
                else
                {
                    // Else use normal movement calculation
                    movement = new Vector2(
                        m_movementInput.x * m_characterData.m_moveSpeed * m_movementMultiplier * Time.deltaTime,
                        m_rigidBody.velocity.y);
                }
                // Update movement
                m_rigidBody.velocity = movement;
            }

            // If dashing is true
            if (m_dashing)
            {
                // Increase dash timer
                m_dashingTimer += Time.deltaTime;

                // If dashing right
                if (m_dashingRight)
                {
                    // Increase the velocity in the right direction
                    m_rigidBody.velocity = Vector2.right * m_dashForce * Time.deltaTime;
                }

                // If dashing left
                if (m_dashingLeft)
                {
                    // Increase the velocity in the left direction
                    m_rigidBody.velocity = Vector2.left * m_dashForce * Time.deltaTime;
                }

                // If the dashing timer is greater than or equal the dash time
                if (m_dashingTimer >= m_dashTime)
                {
                    // Set the timer to 0
                    m_dashingTimer = 0;

                    // Set the bools to false
                    m_dashing = false;
                    m_dashingRight = false;
                    m_dashingLeft = false;

                    // Set the player's velocity magnitude to 0 by adding
                    // the inverse of the current velocity
                    Vector2 inverseVelocity = -m_rigidBody.velocity;
                    m_rigidBody.velocity += inverseVelocity;
                }
            }

            // If the player is holding down the jump button
            if (m_heldJumping)
            {
                // If jump timer is greater than 0
                if (m_jumpTimer > 0)
                {
                    // Set the velocity to increase upwards if not wall jumping
                    if (!m_wallJumpLeft && !m_wallJumpRight)
                    {
                        m_rigidBody.velocity = Vector2.up * m_characterData.m_jumpSpeed * m_movementMultiplier *
                                               Time.deltaTime;
                    }
                    // Set the velocity to increase upwards and to the right if wall jumping left
                    else if (m_wallJumpLeft && !m_wallJumpRight)
                    {
                        m_rigidBody.velocity = new Vector2(1, 1) * m_characterData.m_jumpSpeed * m_movementMultiplier *
                                               Time.deltaTime;
                    }
                    // Set the velocity to increase upwards and to the left if wall jumping right
                    else if (!m_wallJumpLeft && m_wallJumpRight)
                    {
                        m_rigidBody.velocity = new Vector2(-1, 1) * m_characterData.m_jumpSpeed * m_movementMultiplier *
                                               Time.deltaTime;
                    }

                    // Decrease the jump timer
                    m_jumpTimer -= Time.deltaTime;
                }

                // else set jumping to false
                else
                {
                    m_jumping = false;
                    m_heldJumping = false;
                }
            }
        }
    }

    /// <summary>
    /// Makes the player jump in the air
    /// </summary>
    private void Jump()
    {
        // Check if there is a wall to the left
        bool wallCheckLeft = Physics2D.OverlapCircle(m_leftWallCheck.position, m_wallCheckRadius * 3, m_wallMask);
        // Check if there is a wall to the right
        bool wallCheckRight = Physics2D.OverlapCircle(m_rightWallCheck.position, m_wallCheckRadius * 3, m_wallMask);
        // Check if the player is grounded
        bool grounded = Physics2D.OverlapCircle(m_groundCheck.position, m_groundCheckRadius, m_wallMask);
        // Check if coyote jump is possible from the left
        bool coyoteCheckLeft = Physics2D.OverlapCircle(m_leftCoyoteCheck.position, m_coyoteCheckRadius, m_wallMask);
        // Check if coyote jump is possible from the right
        bool coyoteCheckRight = Physics2D.OverlapCircle(m_rightCoyoteCheck.position, m_coyoteCheckRadius, m_wallMask);

        // If player is grounded then jump
        if (grounded)
        {
            // Set jumping to true
            m_jumping = true;
            // Set the jump timer
            m_jumpTimer = m_jumpTime;
            // Set the wall jump bools to false
            m_wallJumpLeft = false;
            m_wallJumpRight = false;
            // If the player is moving right jump upwards and to the right
            if (m_rigidBody.velocity.x > 0)
            {
                m_rigidBody.velocity = new Vector2(1, 1) * m_characterData.m_jumpSpeed * m_movementMultiplier;
            }
            // If the player is moving left jump upwards and to the left
            else if (m_rigidBody.velocity.x < 0)
            {
                m_rigidBody.velocity = new Vector2(-1, 1) * m_characterData.m_jumpSpeed * m_movementMultiplier;
            }
            // Else jump straight upwards
            else
            {
                m_rigidBody.velocity = Vector2.up * m_characterData.m_jumpSpeed * m_movementMultiplier;
            }
        }
        // If wall jumping right or coyote jumping right is possible
        else if (wallCheckRight || coyoteCheckRight)
        {
            // Set jumping to true
            m_jumping = true;
            // Set the jump timer
            m_jumpTimer = m_jumpTime;
            // Set wall jump right to true and wall jump left to false
            m_wallJumpLeft = false;
            m_wallJumpRight = true;
            // Jump upwards and to the left
            m_rigidBody.velocity = new Vector2(-1, 1) * m_characterData.m_jumpSpeed * m_movementMultiplier;
        }
        // If wall jumping left or coyote jumping left is possible
        else if (wallCheckLeft || coyoteCheckLeft)
        {
            // Set jumping to true
            m_jumping = true;
            // Set the jump timer
            m_jumpTimer = m_jumpTime;
            // Set wall jump right to false and wall jump left to true
            m_wallJumpLeft = true;
            m_wallJumpRight = false;
            // Jump upwards and to the right
            m_rigidBody.velocity = new Vector2(1, 1) * m_characterData.m_jumpSpeed * m_movementMultiplier;
        }

        // Create the jump particle system based on the collision check
        if (grounded)
        {
            GameObject jumpPS = (GameObject)Instantiate(m_jumpPSPrefab, m_groundCheck.position, Quaternion.Euler(0, 0, 0));
        }
        if (wallCheckRight)
        {
            GameObject jumpPS = (GameObject)Instantiate(m_jumpPSPrefab, m_rightWallCheck.position, Quaternion.Euler(0, 0, 90));
        }
        if (wallCheckLeft)
        {
            GameObject jumpPS = (GameObject)Instantiate(m_jumpPSPrefab, m_leftWallCheck.position, Quaternion.Euler(0, 0, -90));
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
            if (!GameManager.Instance.Finished)
            {
                GameManager.Instance.ChangeState(GameState.LevelOver);
            }
        }
        // If collectable then increase the score and destroy collectable
        if (other.tag == "Collectable")
        {
            // Collect the collectable and add collectable value to score
            Collectable collectable = other.gameObject.GetComponent<Collectable>();
            m_score += collectable.Collect();

            // Update the level data score
            GameManager.Instance.UpdateCurrentLevelScore(m_score);

            // Destroy the collectable object
            Destroy(other.gameObject);
        }

        // If the player collided with a room entrance
        if (other.tag == "Entrance")
        {
            // Find the room exit class
            RoomExit exit = other.gameObject.GetComponent<RoomExit>();

            // Check if the exit has a room
            if (exit.GetRoom() != null)
            {
                // If there is a room, get it's transform
                Transform targetTransform = exit.GetRoom().transform;

                // Set the players current room
                m_currentRoom = exit.GetRoom();

                // Send data to the database
                GameManager.Instance.UpdateDataOnDatabase();
            }
        }

        // Check if the player collided with an obstacle
        if (other.tag == "Obstacle" && !GameManager.Instance.Finished)
        {
            // Remove a life from the player
            m_currentHealth--;

            // Update the player data with new life count
            GameManager.Instance.UpdateCurrentLevelLives(m_currentHealth);

            // Set the player position to the start position
            transform.position = m_startPosition;

            // Set all dashing bools to false
            m_dashing = false;
            m_dashingLeft = false;
            m_dashingRight = false;

            // Set gamestate to game over
            GameManager.Instance.ChangeState(GameState.GameOver);

            // Set the player's velocity magnitude to 0 by adding
            // the inverse of the current velocity
            Vector2 velocity = m_rigidBody.velocity;
            m_rigidBody.velocity += -velocity;

            // Disable the renderer
            m_renderer.enabled = false;
        }

        // Check if the player hits a one way trigger
        if (other.tag == "OneWayTrigger")
        {
            // Get the one way platform class
            OneWayPlatform oneWayPlatform = other.gameObject.GetComponent<OneWayPlatform>();

            // Disable the solid collider
            oneWayPlatform.DisableSolidCollider();

            // Start enable after coroutine
            oneWayPlatform.StartCoroutine(oneWayPlatform.EnableAfter());
        }
    }

    /// <summary>
    /// Reset the player
    /// </summary>
    public void ResetPlayer()
    {
        // Set the player's position to the start position
        transform.position = m_startPosition;

        // Set the player's velocity magnitude to 0 by adding
        // the inverse of the current velocity
        Vector2 velocity = m_rigidBody.velocity;
        m_rigidBody.velocity += -velocity;

        // Enable the sprite renderer
        m_renderer.enabled = true;
    }

    /// <summary>
    /// Freeze the player
    /// </summary>
    public void Freeze()
    {
        // Get the current velocity of the player
        m_pausedVelocity = m_rigidBody.velocity;

        // Set the player's velocity magnitude to 0 by adding
        // the inverse of the current velocity
        m_rigidBody.velocity += -m_pausedVelocity;

        // Set the gravity scale to 0
        m_rigidBody.gravityScale = 0;
    }

    /// <summary>
    /// Unfreeze the player
    /// </summary>
    public void UnFreeze()
    {
        // Add the original velocity before the player froze
        m_rigidBody.velocity += m_pausedVelocity;

        // Set the gravity scale back to 1
        m_rigidBody.gravityScale = 1;
    }

    /// <summary>
    /// Set the current room the player is in
    /// </summary>
    /// <param name="_newRoom">New current room</param>
    public void SetCurrentRoom(Room _newRoom)
    {
        m_currentRoom = _newRoom;
    }

    /// <summary>
    /// Set the spawn position
    /// </summary>
    /// <param name="_position">Spawn positon</param>
    public void SetSpawn(Vector3 _position)
    {
        m_startPosition = _position;
    }

    /// <summary>
    /// Get the current room the player is in
    /// </summary>
    /// <returns>The current room the player is in</returns>
    public Room GetCurrentRoom()
    {
        return m_currentRoom;
    }

    /// <summary>
    /// Update the players sprite
    /// </summary>
    protected override void UpdateSprite()
    {
        // If the x velocity is 0 set sprite to idle
        if (m_rigidBody.velocity.x == 0)
        {
            m_renderer.sprite = m_characterData.m_idleSprite;
        }

        // If the x velocity is greater than 0 set sprite to right facing sprite
        else if (m_rigidBody.velocity.x > 0 && !m_sprinting)
        {
            m_renderer.sprite = m_characterData.m_rightSprite;
        }
        // Else x velocity is less than 0
        // Set sprite to left facing sprite
        else if (m_rigidBody.velocity.x < 0 && !m_sprinting)
        {
            m_renderer.sprite = m_characterData.m_leftSprite;
        }

        // If the x velocity is greater than 0 set sprite to right facing sprite
        else if (m_rigidBody.velocity.x > 0 && m_sprinting)
        {
            m_renderer.sprite = m_sprintRightSprite;
        }
        // Else x velocity is less than 0
        // Set sprite to left facing sprite
        else if (m_rigidBody.velocity.x < 0 && m_sprinting)
        {
            m_renderer.sprite = m_sprintLeftSprite;
        }
    }
}