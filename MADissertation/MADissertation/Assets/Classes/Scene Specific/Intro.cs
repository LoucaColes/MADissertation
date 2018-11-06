using UnityEngine;

public class Intro : MonoBehaviour
{
    // Designer variables
    [SerializeField]
    private SceneData m_gameSceneData;

    /// <summary>
    /// Load the game
    /// </summary>
    public void LoadGame()
    {
        SceneLoader.Instance.LoadScene(m_gameSceneData.m_sceneName);
    }
}