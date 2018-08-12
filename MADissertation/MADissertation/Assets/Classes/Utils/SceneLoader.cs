using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

/// <summary>
/// Singleton class that can be used to load other scenes
/// </summary>
public class SceneLoader : Singleton<SceneLoader>
{
    /// <summary>
    /// Load a scene
    /// </summary>
    /// <param name="_sceneName">Scene Name</param>
    public void LoadScene(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);
    }

    /// <summary>
    /// Load a scene
    /// </summary>
    /// <param name="_sceneIndex">Scene Index</param>
    public void LoadScene(int _sceneIndex)
    {
        SceneManager.LoadScene(_sceneIndex);
    }

    /// <summary>
    /// Load a scene using A-sync
    /// </summary>
    /// <param name="_sceneName">Scene Name</param>
    public void LoadSceneASync(string _sceneName)
    {
        SceneManager.LoadSceneAsync(_sceneName);
    }

    /// <summary>
    /// Load a scene using A-sync
    /// </summary>
    /// <param name="_sceneIndex">Scene Index</param>
    public void LoadSceneASync(int _sceneIndex)
    {
        SceneManager.LoadSceneAsync(_sceneIndex);
    }

    /// <summary>
    /// Exit the game
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}