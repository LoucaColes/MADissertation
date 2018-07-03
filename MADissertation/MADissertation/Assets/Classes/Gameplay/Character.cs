using System.Collections;
using System.Collections.Generic;
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
        m_currentHealth = m_characterData.m_health;
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_renderer = GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
    protected virtual void Start()
    {
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateSprite();
    }

    protected virtual void FixedUpdate()
    {
    }

    private void UpdateSprite()
    {
        if (m_rigidBody.velocity.x == 0)
        {
            m_renderer.sprite = m_characterData.m_idleSprite;
        }
        else if (m_rigidBody.velocity.x > 0)
        {
            m_renderer.sprite = m_characterData.m_rightSprite;
        }
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