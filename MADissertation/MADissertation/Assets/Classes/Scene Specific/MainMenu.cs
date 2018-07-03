using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private SceneData m_gameSceneData;

    [SerializeField]
    private SceneData m_aboutSceneData;

    [SerializeField]
    private string m_questionnaireLink;

    public void LoadGame()
    {
        SceneLoader.Instance.LoadScene(m_gameSceneData.m_sceneName);
    }

    public void LoadAbout()
    {
        SceneLoader.Instance.LoadScene(m_aboutSceneData.m_sceneName);
    }

    public void Exit()
    {
        SceneLoader.Instance.ExitGame();
    }

    public void LoadQuestionnaire()
    {
        Application.OpenURL(m_questionnaireLink);
    }
}

[System.Serializable]
public struct SceneData
{
    public string m_sceneName;
    public int m_sceneIndex;
    public bool m_loadASync;
}