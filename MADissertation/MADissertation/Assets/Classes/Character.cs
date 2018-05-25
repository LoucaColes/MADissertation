using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    protected CharacterStartData m_characterData;

    [SerializeField]
    protected float m_gravity;

    protected int m_currentHealth;
    protected Rigidbody2D m_rigidBody;

    protected virtual void Awake()
    {
        m_currentHealth = m_characterData.m_health;
        m_rigidBody = GetComponent<Rigidbody2D>();
    }

    // Use this for initialization
    protected virtual void Start()
    {
    }

    // Update is called once per frame
    protected virtual void Update()
    {
    }

    protected virtual void FixedUpdate()
    {
    }
}

// Data to initialise the character with
[System.Serializable]
public struct CharacterStartData
{
    public int m_health;
    public float m_moveSpeed;
    public float m_jumpSpeed;
}