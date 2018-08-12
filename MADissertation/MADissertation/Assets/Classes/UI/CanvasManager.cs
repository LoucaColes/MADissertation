using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A manager class that allows other classes to
/// access diffent canvases
/// </summary>
public class CanvasManager : MonoBehaviour
{
    // Designer variables
    [SerializeField]
    private GameObject m_gameCanvas;

    [SerializeField]
    private GameObject m_pauseCanvas;

    [SerializeField]
    private GameObject m_respawningText;

    [SerializeField]
    private GameCanvas m_gameCanvasClass;

    /// <summary>
    /// Enable the game canvas
    /// </summary>
    public void EnableGameCanvas()
    {
        m_gameCanvas.SetActive(true);
        m_pauseCanvas.SetActive(false);
    }

    /// <summary>
    /// Enable the pause canvas
    /// </summary>
    public void EnablePauseCanvas()
    {
        m_gameCanvas.SetActive(false);
        m_pauseCanvas.SetActive(true);
    }

    /// <summary>
    /// Exit to the main menu
    /// </summary>
    public void Exit()
    {
        SceneLoader.Instance.LoadScene(0);
    }

    /// <summary>
    /// Access the respawning text object
    /// </summary>
    /// <returns>Respawning text object</returns>
    public GameObject GetRespawningText()
    {
        return m_respawningText;
    }

    /// <summary>
    /// Access the game canvas
    /// </summary>
    /// <returns>Game Canvas object</returns>
    public GameCanvas GetGameCanvas()
    {
        return m_gameCanvasClass;
    }
}