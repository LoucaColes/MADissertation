using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class ControlScheme : MonoBehaviour
{
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

    // Use this for initialization
    private void Start()
    {
        if (ReInput.controllers.joystickCount > 0)
        {
            string filePath = Application.dataPath + "/" + m_fileName + ".txt";
            File.WriteAllText(filePath, ReInput.controllers.Joysticks[0].hardwareName);
        }
        else
        {
            string filePath = Application.dataPath + "/" + m_fileName + ".txt";
            File.WriteAllText(filePath, "Keyboard");
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (ReInput.controllers.joystickCount == 0)
        {
            EnableKeyboardInput();
        }
        else
        {
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
            Debug.Log(ReInput.controllers.Joysticks[0].hardwareName);
        }
    }

    public void LoadGame()
    {
        SceneLoader.Instance.LoadScene(m_gameSceneData.m_sceneName);
    }

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

[Serializable]
public struct InputSchemeObjects
{
    public string m_name;
    public GameObject m_gameplayInput;
    public GameObject m_uiInput;
}