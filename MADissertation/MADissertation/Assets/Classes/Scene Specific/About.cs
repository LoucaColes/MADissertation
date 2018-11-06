using UnityEngine;

/// <summary>
/// Class that handles the about scene
/// </summary>
public class About : MonoBehaviour
{
    // Main menu scene data
    [SerializeField]
    private SceneData m_mainSceneData;

    /// <summary>
    /// Load the main menu scene
    /// </summary>
    public void LoadMain()
    {
        SceneLoader.Instance.LoadScene(m_mainSceneData.m_sceneName);
    }
}