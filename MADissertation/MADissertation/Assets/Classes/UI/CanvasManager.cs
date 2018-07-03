using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    // Designer variables
    [SerializeField]
    private GameObject m_gameCanvas;

    [SerializeField]
    private GameObject m_pauseCanvas;

    [SerializeField]
    private GameObject m_respawningText;

    public void EnableGameCanvas()
    {
        m_gameCanvas.SetActive(true);
        m_pauseCanvas.SetActive(false);
    }

    public void EnablePauseCanvas()
    {
        m_gameCanvas.SetActive(false);
        m_pauseCanvas.SetActive(true);
    }

    public void Exit()
    {
        SceneLoader.Instance.LoadScene(0);
    }

    public GameObject GetRespawningText()
    {
        return m_respawningText;
    }
}