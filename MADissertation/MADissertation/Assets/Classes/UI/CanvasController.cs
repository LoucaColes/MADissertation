using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class CanvasController : MonoBehaviour
{
    [SerializeField]
    private int m_playerId;

    [SerializeField]
    private float m_inputDelay;

    [SerializeField]
    private Button[] m_buttons;

    private Rewired.Player m_rewiredPlayer;
    private int m_buttonIndex = 0;
    private float m_inputDelayTimer = 0;

    // Use this for initialization
    private void Start()
    {
        m_rewiredPlayer = ReInput.players.GetPlayer(m_playerId);
        UpdateButtons();
    }

    // Update is called once per frame
    private void Update()
    {
        m_inputDelayTimer += Time.deltaTime;
        if (m_rewiredPlayer.GetAxis("MoveVertical") > 0 && m_inputDelayTimer > m_inputDelay)
        {
            m_buttonIndex++;
            if (m_buttonIndex > m_buttons.Length - 1)
            {
                m_buttonIndex = 0;
            }
            UpdateButtons();
            m_inputDelayTimer = 0;
        }
        else if (m_rewiredPlayer.GetAxis("MoveVertical") < 0 && m_inputDelayTimer > m_inputDelay)
        {
            m_buttonIndex--;
            if (m_buttonIndex < 0)
            {
                m_buttonIndex = m_buttons.Length - 1;
            }
            UpdateButtons();
            m_inputDelayTimer = 0;
        }
        if (m_rewiredPlayer.GetButtonDown("Jump"))
        {
        }
    }

    private void UpdateButtons()
    {
        for (int index = 0; index < m_buttons.Length; index++)
        {
            if (index != m_buttonIndex)
            {
                m_buttons[index].image.color = m_buttons[index].colors.disabledColor;
            }
            else
            {
                m_buttons[index].image.color = m_buttons[index].colors.highlightedColor;
            }
        }
    }
}