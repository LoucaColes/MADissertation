using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

/// <summary>
/// Manages game and its states
/// </summary>
public class GameManager : Singleton<GameManager>
{
    // Designer variables
    [SerializeField]
    private GameObject m_sceneLoaderPrefab;

    private GameObject m_respawningText;

    [SerializeField]
    private bool m_debugMode = false;

    // Private variables
    private GameState m_gameState = GameState.Init;

    private PlayerData m_currentPlayerData;
    private CanvasManager m_canvasManager = null;

    // Use this for initialization
    private void Start()
    {
        ChangeState(GameState.Init);
    }

    // Update is called once per frame
    private void Update()
    {
        switch (m_gameState)
        {
            case GameState.Init:
                break;

            case GameState.Play:
                m_currentPlayerData.m_timeTakenToCompleteLevel += Time.deltaTime;
                if (m_debugMode)
                {
                    if (Input.GetKeyDown(KeyCode.T))
                    {
                        ChangeState(GameState.LevelOver);
                    }
                }
                break;

            case GameState.Pause:
                break;

            case GameState.GameOver:
                break;

            case GameState.LevelOver:
                break;
        }
    }

    public void ChangeState(GameState _newGameState)
    {
        m_gameState = _newGameState;
        switch (m_gameState)
        {
            case GameState.Init:
                m_currentPlayerData = new PlayerData();
                m_currentPlayerData.m_currentPlayerLives = 0;
                m_currentPlayerData.m_currentPlayerDeaths = 0;
                m_currentPlayerData.m_enemiesKilled = 0;
                m_currentPlayerData.m_timeTakenToCompleteLevel = 0;
                m_currentPlayerData.m_score = 0;

                if (!SceneLoader.IsInitialized)
                {
                    Debug.LogError("No Scene Loader");
                    Instantiate(m_sceneLoaderPrefab, Vector3.zero, Quaternion.identity);
                }

                m_canvasManager = GameObject.FindObjectOfType<CanvasManager>();
                m_respawningText = m_canvasManager.GetRespawningText();
                m_respawningText.SetActive(false);

                Player initPlayer = GameObject.FindObjectOfType<Player>();
                if (initPlayer != null)
                {
                    initPlayer.ResetPlayer();
                }

                CameraMovement cameraMovement = Camera.main.gameObject.GetComponent<CameraMovement>();
                if (initPlayer != null)
                {
                    cameraMovement.ResetCamera();
                }

                ChangeState(GameState.Play);
                break;

            case GameState.Play:
                Player player = GameObject.FindObjectOfType<Player>();
                if (player != null)
                {
                    player.UnFreeze();
                }
                m_canvasManager.EnableGameCanvas();
                break;

            case GameState.Pause:
                Player pausedPlayer = GameObject.FindObjectOfType<Player>();
                pausedPlayer.Freeze();
                m_canvasManager.EnablePauseCanvas();
                break;

            case GameState.GameOver:
                CameraMovement tempCameraMovement = Camera.main.gameObject.GetComponent<CameraMovement>();
                tempCameraMovement.StopAllCoroutines();
                m_respawningText.SetActive(true);
                m_currentPlayerData.m_currentPlayerDeaths++;
                StartCoroutine(ResetAfterGameOver());
                break;

            case GameState.LevelOver:
                DataTracker.Instance.AddNewPlayerData(ref m_currentPlayerData);
                m_currentPlayerData = new PlayerData();
                StartCoroutine(LoadMain());
                break;
        }
    }

    private IEnumerator LoadMain()
    {
        yield return new WaitForSeconds(1f);
        DataTracker.Instance.RandomiseDifficulty();
        SceneLoader.Instance.LoadScene(5);
    }

    public void ChangeState(int _newGameState)
    {
        m_gameState = (GameState)_newGameState;
        switch (m_gameState)
        {
            case GameState.Init:
                m_currentPlayerData = new PlayerData();
                m_currentPlayerData.m_currentPlayerLives = 0;
                m_currentPlayerData.m_currentPlayerDeaths = 0;
                m_currentPlayerData.m_enemiesKilled = 0;
                m_currentPlayerData.m_timeTakenToCompleteLevel = 0;
                m_currentPlayerData.m_score = 0;

                m_canvasManager = GameObject.FindObjectOfType<CanvasManager>();

                ChangeState(GameState.Play);
                break;

            case GameState.Play:
                Player player = GameObject.FindObjectOfType<Player>();
                player.UnFreeze();
                m_canvasManager.EnableGameCanvas();
                break;

            case GameState.Pause:
                Player pausedPlayer = GameObject.FindObjectOfType<Player>();
                pausedPlayer.Freeze();
                m_canvasManager.EnablePauseCanvas();
                break;

            case GameState.GameOver:
                StartCoroutine(ResetAfterGameOver());
                break;

            case GameState.LevelOver:
                DataTracker.Instance.AddNewPlayerData(ref m_currentPlayerData);
                m_currentPlayerData = new PlayerData();
                break;
        }
    }

    private IEnumerator ResetAfterGameOver()
    {
        yield return new WaitForSeconds(5f);
        ChangeState(GameState.Init);
    }

    public void UpdateCurrentLevelLives(int _lives)
    {
        m_currentPlayerData.m_currentPlayerLives = _lives;
    }

    public void UpdateCurrentLevelScore(int _score)
    {
        m_currentPlayerData.m_score = _score;
    }

    public GameState GetGameState()
    {
        return m_gameState;
    }

    public bool IsDebug()
    {
        return m_debugMode;
    }
}

public enum GameState
{
    Init,
    Play,
    Pause,
    GameOver,
    LevelOver,
}