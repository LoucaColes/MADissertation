using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

/// <summary>
/// A class that controls the control scheme scene
/// </summary>
public class ControlScheme : MonoBehaviour
{
    // Designer variables
    [SerializeField]
    private SceneData m_gameSceneData;

    [SerializeField]
    private InputSchemeObjects m_keyboardInput;

    [SerializeField]
    private InputSchemeObjects m_xbox360Input;

    [SerializeField]
    private InputSchemeObjects m_xboxOneInput;

    [SerializeField]
    private InputSchemeObjects m_ps3Input;

    [SerializeField]
    private InputSchemeObjects m_ps4Input;

    [SerializeField]
    private GameObject m_defaultText;

    [SerializeField]
    private string m_fileName = "HardwareName";

    // Update is called once per frame
    private void Update()
    {
        // Check if there is any controllers connected
        // If there isn't any then enable keyboard input text
        if (ReInput.controllers.joystickCount == 0)
        {
            EnableKeyboardInput();
        }
        else
        {
            // Dependant on the controller name, enable related
            // Input text
            switch (ReInput.controllers.Joysticks[0].name)
            {
                case "XInput Gamepad 1":
                    EnableXOneInput();
                    break;

                case "Sony DuelShock 4":
                    EnablePS4Input();
                    break;

                case "Sony DuelShock 3":
                    EnablePS3Input();
                    break;

                default:
                    DisableAllInput();
                    break;
            }
        }
    }

    /// <summary>
    /// Load the game scene
    /// </summary>
    public void LoadGame()
    {
        SceneLoader.Instance.LoadScene(m_gameSceneData.m_sceneName);
    }

    /// <summary>
    /// Enable the keyboard input text
    /// </summary>
    private void EnableKeyboardInput()
    {
        m_keyboardInput.m_gameplayInput.SetActive(true);
        m_keyboardInput.m_uiInput.SetActive(true);

        m_xbox360Input.m_gameplayInput.SetActive(false);
        m_xbox360Input.m_uiInput.SetActive(false);

        m_xboxOneInput.m_gameplayInput.SetActive(false);
        m_xboxOneInput.m_uiInput.SetActive(false);

        m_ps3Input.m_gameplayInput.SetActive(false);
        m_ps3Input.m_uiInput.SetActive(false);

        m_ps4Input.m_gameplayInput.SetActive(false);
        m_ps4Input.m_uiInput.SetActive(false);

        m_defaultText.SetActive(false);
    }

    /// <summary>
    /// Enable the xbox 360 input text
    /// </summary>
    private void EnableX360Input()
    {
        m_keyboardInput.m_gameplayInput.SetActive(false);
        m_keyboardInput.m_uiInput.SetActive(false);

        m_xbox360Input.m_gameplayInput.SetActive(true);
        m_xbox360Input.m_uiInput.SetActive(true);

        m_xboxOneInput.m_gameplayInput.SetActive(false);
        m_xboxOneInput.m_uiInput.SetActive(false);

        m_ps3Input.m_gameplayInput.SetActive(false);
        m_ps3Input.m_uiInput.SetActive(false);

        m_ps4Input.m_gameplayInput.SetActive(false);
        m_ps4Input.m_uiInput.SetActive(false);

        m_defaultText.SetActive(false);
    }

    /// <summary>
    /// Enable the Xbox One input text
    /// </summary>
    private void EnableXOneInput()
    {
        m_keyboardInput.m_gameplayInput.SetActive(false);
        m_keyboardInput.m_uiInput.SetActive(false);

        m_xbox360Input.m_gameplayInput.SetActive(false);
        m_xbox360Input.m_uiInput.SetActive(false);

        m_xboxOneInput.m_gameplayInput.SetActive(true);
        m_xboxOneInput.m_uiInput.SetActive(true);

        m_ps3Input.m_gameplayInput.SetActive(false);
        m_ps3Input.m_uiInput.SetActive(false);

        m_ps4Input.m_gameplayInput.SetActive(false);
        m_ps4Input.m_uiInput.SetActive(false);

        m_defaultText.SetActive(false);
    }

    /// <summary>
    /// Enable the ps3 input text
    /// </summary>
    private void EnablePS3Input()
    {
        m_keyboardInput.m_gameplayInput.SetActive(false);
        m_keyboardInput.m_uiInput.SetActive(false);

        m_xbox360Input.m_gameplayInput.SetActive(false);
        m_xbox360Input.m_uiInput.SetActive(false);

        m_xboxOneInput.m_gameplayInput.SetActive(false);
        m_xboxOneInput.m_uiInput.SetActive(false);

        m_ps3Input.m_gameplayInput.SetActive(true);
        m_ps3Input.m_uiInput.SetActive(true);

        m_ps4Input.m_gameplayInput.SetActive(false);
        m_ps4Input.m_uiInput.SetActive(false);

        m_defaultText.SetActive(false);
    }

    /// <summary>
    /// Enable the ps4 input text
    /// </summary>
    private void EnablePS4Input()
    {
        m_keyboardInput.m_gameplayInput.SetActive(false);
        m_keyboardInput.m_uiInput.SetActive(false);

        m_xbox360Input.m_gameplayInput.SetActive(false);
        m_xbox360Input.m_uiInput.SetActive(false);

        m_xboxOneInput.m_gameplayInput.SetActive(false);
        m_xboxOneInput.m_uiInput.SetActive(false);

        m_ps3Input.m_gameplayInput.SetActive(false);
        m_ps3Input.m_uiInput.SetActive(false);

        m_ps4Input.m_gameplayInput.SetActive(true);
        m_ps4Input.m_uiInput.SetActive(true);

        m_defaultText.SetActive(false);
    }

    /// <summary>
    /// Disable all input text
    /// </summary>
    private void DisableAllInput()
    {
        m_keyboardInput.m_gameplayInput.SetActive(false);
        m_keyboardInput.m_uiInput.SetActive(false);

        m_xbox360Input.m_gameplayInput.SetActive(false);
        m_xbox360Input.m_uiInput.SetActive(false);

        m_xboxOneInput.m_gameplayInput.SetActive(false);
        m_xboxOneInput.m_uiInput.SetActive(false);

        m_ps3Input.m_gameplayInput.SetActive(false);
        m_ps3Input.m_uiInput.SetActive(false);

        m_ps4Input.m_gameplayInput.SetActive(false);
        m_ps4Input.m_uiInput.SetActive(false);

        m_defaultText.SetActive(true);
    }
}

/// <summary>
/// A struct containing the input name and various input text objects
/// </summary>
[Serializable]
public struct InputSchemeObjects
{
    public string m_name;
    public GameObject m_gameplayInput;
    public GameObject m_uiInput;
}