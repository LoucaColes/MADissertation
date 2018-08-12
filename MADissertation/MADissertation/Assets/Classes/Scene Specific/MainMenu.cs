using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that controls the main menu
/// </summary>
public class MainMenu : MonoBehaviour
{
    // Designer variables
    [SerializeField]
    private SceneData m_gameSceneData;

    [SerializeField]
    private SceneData m_aboutSceneData;

    [SerializeField]
    private string m_questionnaireLink;

    /// <summary>
    /// Load the game
    /// </summary>
    public void LoadGame()
    {
        SceneLoader.Instance.LoadScene(m_gameSceneData.m_sceneName);
    }

    /// <summary>
    /// Load the about page
    /// </summary>
    public void LoadAbout()
    {
        SceneLoader.Instance.LoadScene(m_aboutSceneData.m_sceneName);
    }

    /// <summary>
    /// Exit the game
    /// </summary>
    public void Exit()
    {
        SceneLoader.Instance.ExitGame();
    }

    /// <summary>
    /// Load the questionnaire
    /// </summary>
    public void LoadQuestionnaire()
    {
        Application.OpenURL(m_questionnaireLink);
    }
}

/// <summary>
/// Struct that stores various scene data values
/// </summary>
[System.Serializable]
public struct SceneData
{
    public string m_sceneName;
    public int m_sceneIndex;
    public bool m_loadASync;
}