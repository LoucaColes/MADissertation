using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

public class SceneLoader : Singleton<SceneLoader>
{
    public void LoadScene(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);
    }

    public void LoadScene(int _sceneIndex)
    {
        SceneManager.LoadScene(_sceneIndex);
    }

    public void LoadSceneASync(string _sceneName)
    {
        SceneManager.LoadSceneAsync(_sceneName);
    }

    public void LoadSceneASync(int _sceneIndex)
    {
        SceneManager.LoadSceneAsync(_sceneIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}