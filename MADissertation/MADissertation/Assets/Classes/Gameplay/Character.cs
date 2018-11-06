using UnityEngine;

/// <summary>
/// Base class for player and enemies
/// </summary>
public class Character : MonoBehaviour
{
    // Designer Variables
    [SerializeField]
    protected CharacterStartData m_characterData;

    [SerializeField]
    protected float m_gravity;

    // Protected variables
    protected int m_currentHealth;
    protected Rigidbody2D m_rigidBody;
    protected SpriteRenderer m_renderer;

    /// <summary>
    /// Handles inital set up
    /// </summary>
    protected virtual void Awake()
    {
        // Set the current health
        m_currentHealth = m_characterData.m_health;

        // Get the rigidbody
        m_rigidBody = GetComponent<Rigidbody2D>();

        // Get the sprite renderer
        m_renderer = GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
    protected virtual void Start()
    {
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        // Update the character sprite
        UpdateSprite();
    }

    /// <summary>
    /// Fixed update is called once per fixed frame
    /// </summary>
    protected virtual void FixedUpdate()
    {
    }

    /// <summary>
    /// Update the sprite based on the characters velocity
    /// </summary>
    protected virtual void UpdateSprite()
    {
        // If the x velocity is 0 set sprite to idle
        if (m_rigidBody.velocity.x == 0)
        {
            m_renderer.sprite = m_characterData.m_idleSprite;
        }

        // If the x velocity is greater than 0 set sprite to right facing sprite
        else if (m_rigidBody.velocity.x > 0)
        {
            m_renderer.sprite = m_characterData.m_rightSprite;
        }
        // Else x velocity is less than 0
        // Set sprite to left facing sprite
        else
        {
            m_renderer.sprite = m_characterData.m_leftSprite;
        }
    }
}

/// <summary>
/// Data to initialise the character with
/// </summary>
[System.Serializable]
public struct CharacterStartData
{
    public int m_health;
    public float m_moveSpeed;
    public float m_jumpSpeed;
    public Sprite m_idleSprite;
    public Sprite m_rightSprite;
    public Sprite m_leftSprite;
}