using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Player : Character
{
    [SerializeField]
    private int m_playerId = 0;

    [SerializeField]
    private float m_sprintMultiplier = 2f;

    [SerializeField]
    private float m_heightOffset = 0.1f;

    [SerializeField]
    private float m_raycastLength = 0.1f;

    private Rewired.Player m_rewiredPlayer;
    private Vector3 m_movementInput;
    private bool m_sprinting = false;
    private float m_movementMultiplier = 1;
    private Collider2D m_collider;
    private float m_height;
    private bool m_grounded = false;
    private int m_score;

    /// <summary>
    /// Handle Set Up
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        m_rewiredPlayer = ReInput.players.GetPlayer(0);
        m_collider = GetComponent<Collider2D>();
        m_height = m_collider.bounds.extents.y + m_heightOffset;
    }

    // Use this for initialization
    private void Start()
    {
    }

    /// <summary>
    /// Handle Input
    /// </summary>
    // Update is called once per frame
    protected override void Update()
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
    }

    /// <summary>
    /// Handle physics
    /// </summary>
    protected override void FixedUpdate()
    {
        Vector2 movement;
        if (!m_grounded)
        {
            movement = new Vector2(m_movementInput.x * m_characterData.m_moveSpeed * m_movementMultiplier, m_gravity);
        }
        else
        {
            movement = new Vector2(m_movementInput.x * m_characterData.m_moveSpeed * m_movementMultiplier, 0);
        }

        // Update movement
        m_rigidBody.AddForce(movement * Time.deltaTime);
    }

    private void Jump()
    {
        if (m_grounded)
        {
            Vector2 movement = new Vector2(m_movementInput.x * m_characterData.m_moveSpeed * m_movementMultiplier,
                m_characterData.m_jumpSpeed);
            m_rigidBody.AddForce(movement * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "EndZone")
        {
            Debug.Log("Hit End Zone");
        }
        if (other.tag == "Collectable")
        {
            Debug.Log("Hit Collectable");
            Collectable collectable = other.gameObject.GetComponent<Collectable>();
            m_score += collectable.Collect();
            Destroy(other.gameObject);
        }
    }
}