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

    [SerializeField]
    private bool m_debugMode = false;

    [SerializeField]
    private int m_levelLimit = 10;

    // Private variables
    private GameState m_gameState = GameState.Init;
    private GameObject m_respawningText;
    private PlayerData m_currentPlayerData;
    private CanvasManager m_canvasManager = null;

    // Use this for initialization
    private void Start()
    {
        // Change state to initialise
        ChangeState(GameState.Init);
    }

    // Update is called once per frame
    private void Update()
    {
        // Update the game based on the current game state
        switch (m_gameState)
        {
            // If state is initialise
            case GameState.Init:
                break;

            // If state is play
            case GameState.Play:
                // Increase the time taken
                m_currentPlayerData.m_timeTakenToCompleteLevel += Time.deltaTime;

                // Update game canvas with new time
                if (m_canvasManager)
                {
                    m_canvasManager.GetGameCanvas().UpdateTimeText(m_currentPlayerData.m_timeTakenToCompleteLevel);
                }

                // If in debug mode
                if (m_debugMode)
                {
                    // If T pressed change state to level over and load next level
                    if (Input.GetKeyDown(KeyCode.T))
                    {
                        ChangeState(GameState.LevelOver);
                    }
                }

                // If Alpha 0 pressed, toggle debug mode
                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    m_debugMode = !m_debugMode;
                }
                break;

            // If state is Pause
            case GameState.Pause:
                break;

            // If state is GameOver
            case GameState.GameOver:
                break;

            // If state is Level Over
            case GameState.LevelOver:
                break;
        }
    }

    /// <summary>
    /// Change the game state
    /// </summary>
    /// <param name="_newGameState">New game state</param>
    public void ChangeState(GameState _newGameState)
    {
        // Set game state to new game state
        m_gameState = _newGameState;

        // Update game based on the new state
        switch (m_gameState)
        {
            // If state is initialise
            case GameState.Init:
                // Create new player data
                m_currentPlayerData = new PlayerData();
                m_currentPlayerData.m_currentPlayerLives = 0;
                m_currentPlayerData.m_currentPlayerDeaths = 0;
                m_currentPlayerData.m_enemiesKilled = 0;
                m_currentPlayerData.m_timeTakenToCompleteLevel = 0;
                m_currentPlayerData.m_score = 0;
                m_currentPlayerData.m_deathRoomData = new List<SimpleRoomData>();

                // Check if there is a Scene Loader
                if (!SceneLoader.IsInitialized)
                {
                    // If there is no Scene Loader, create a new one
                    Debug.LogError("No Scene Loader");
                    Instantiate(m_sceneLoaderPrefab, Vector3.zero, Quaternion.identity);
                }

                // Find the canvas manager object
                m_canvasManager = GameObject.FindObjectOfType<CanvasManager>();

                // Access respawn text and set active to false
                m_respawningText = m_canvasManager.GetRespawningText();
                m_respawningText.SetActive(false);

                // Access game canvas and update level text and deaths text
                m_canvasManager.GetGameCanvas().UpdateLevelText(DataTracker.Instance.GetLevelId());
                m_canvasManager.GetGameCanvas().UpdateDeathsText(m_currentPlayerData.m_currentPlayerDeaths);

                // Find the player object
                Player initPlayer = GameObject.FindObjectOfType<Player>();

                // Find the camera movement object
                CameraMovement cameraMovement = Camera.main.gameObject.GetComponent<CameraMovement>();
                if (initPlayer != null)
                {
                    // If there is a player, reset the camera
                    cameraMovement.ResetCamera();
                }

                // Change the state to play
                ChangeState(GameState.Play);
                break;

            // If state is reset
            case GameState.Reset:
                // Find the canvas manager object
                m_canvasManager = GameObject.FindObjectOfType<CanvasManager>();

                // Find the respawning text object and set active to false
                m_respawningText = m_canvasManager.GetRespawningText();
                m_respawningText.SetActive(false);

                // Find the player object
                Player resetPlayer = GameObject.FindObjectOfType<Player>();
                if (resetPlayer != null)
                {
                    // If there is a player, reset the player
                    resetPlayer.ResetPlayer();
                }

                // Find the camera movement object
                CameraMovement rCameraMovement = Camera.main.gameObject.GetComponent<CameraMovement>();
                if (resetPlayer != null)
                {
                    // If there is a player, reset the camera
                    rCameraMovement.ResetCamera();
                }

                // Change the state to play
                ChangeState(GameState.Play);
                break;

            // If state is play
            case GameState.Play:
                // Find the player object
                Player player = GameObject.FindObjectOfType<Player>();
                if (player != null)
                {
                    // If there is a player, unfreeze the player
                    player.UnFreeze();
                }

                // Enable the game canvas
                m_canvasManager.EnableGameCanvas();
                break;

            // If state is pause
            case GameState.Pause:
                // Find the player object
                Player pausedPlayer = GameObject.FindObjectOfType<Player>();

                // Unfreeze the player
                pausedPlayer.Freeze();

                // Enable the pause canvas
                m_canvasManager.EnablePauseCanvas();
                break;

            // If state is game over
            case GameState.GameOver:
                // Find the camera movement object
                CameraMovement tempCameraMovement = Camera.main.gameObject.GetComponent<CameraMovement>();

                // Stop all coroutines in camera movement class
                tempCameraMovement.StopAllCoroutines();

                // Set the respawn text active to false
                m_respawningText.SetActive(true);

                // Increase the player deaths stat
                m_currentPlayerData.m_currentPlayerDeaths++;

                // Update the deaths text with the new stat
                m_canvasManager.GetGameCanvas().UpdateDeathsText(m_currentPlayerData.m_currentPlayerDeaths);

                // Find the player object
                Player deadPlayer = GameObject.FindObjectOfType<Player>();

                // Add the current room the the death room stat
                m_currentPlayerData.m_deathRoomData.Add(RoomParser.ParseRoomToString(deadPlayer.GetCurrentRoom()));

                // Reset the game
                StartCoroutine(ResetAfterGameOver());
                break;

            // If state is level over
            case GameState.LevelOver:
                // Add player data to the data tracker
                DataTracker.Instance.AddNewPlayerData(ref m_currentPlayerData);

                // Send the data to the database and increase the level count
                int levelCount;
                DataTracker.Instance.SetUpNextLevelDataAndSendToDatabase(out levelCount);

                // Create a new player data
                m_currentPlayerData = new PlayerData();

                // If the level count is less then the limit
                if (levelCount < m_levelLimit)
                {
                    // Load the next level
                    StartCoroutine(LoadNextLevel());
                }
                else // Else load the main menu
                {
                    StartCoroutine(LoadMainMenu());
                }
                break;
        }
    }

    /// <summary>
    /// Loads the next level
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadNextLevel()
    {
        // Wait a second
        yield return new WaitForSeconds(1f);

        // Decide the next levels difficulty
        DataTracker.Instance.DecideNextDifficulty();

        // Load the next level
        SceneLoader.Instance.LoadScene(5);
    }

    /// <summary>
    /// Loads the main menu scene
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadMainMenu()
    {
        // Wait a second
        yield return new WaitForSeconds(1f);

        // Load the main menu
        SceneLoader.Instance.LoadScene("MainMenu");
    }

    /// <summary>
    /// Change the game state
    /// </summary>
    /// <param name="_newGameState">New game state</param>
    public void ChangeState(int _newGameState)
    {
        // set the game state to the new game state
        m_gameState = (GameState)_newGameState;

        switch (m_gameState)
        {
            // If the state is initialise
            case GameState.Init:
                // Create new player data
                m_currentPlayerData = new PlayerData();
                m_currentPlayerData.m_currentPlayerLives = 0;
                m_currentPlayerData.m_currentPlayerDeaths = 0;
                m_currentPlayerData.m_enemiesKilled = 0;
                m_currentPlayerData.m_timeTakenToCompleteLevel = 0;
                m_currentPlayerData.m_score = 0;
                m_currentPlayerData.m_deathRoomData = new List<SimpleRoomData>();

                // Find the canvas manager
                m_canvasManager = GameObject.FindObjectOfType<CanvasManager>();

                // Update the level text
                m_canvasManager.GetGameCanvas().UpdateLevelText(DataTracker.Instance.GetLevelId());

                // Update the deaths text
                m_canvasManager.GetGameCanvas().UpdateDeathsText(m_currentPlayerData.m_currentPlayerDeaths);

                // Change the state to play
                ChangeState(GameState.Play);
                break;

            // If the state is play
            case GameState.Play:
                // Find the player object
                Player player = GameObject.FindObjectOfType<Player>();

                // Unfreeze the player
                player.UnFreeze();

                // Enable the game canvas
                m_canvasManager.EnableGameCanvas();
                break;

            // If the state is pause
            case GameState.Pause:
                // Find the player object
                Player pausedPlayer = GameObject.FindObjectOfType<Player>();

                // Freeze the player
                pausedPlayer.Freeze();

                // Enable the pause canvas
                m_canvasManager.EnablePauseCanvas();
                break;

            // If the state is game over
            case GameState.GameOver:
                // Find the player object
                Player deadPlayer = GameObject.FindObjectOfType<Player>();

                // Add the death room to the death room stat
                m_currentPlayerData.m_deathRoomData.Add(RoomParser.ParseRoomToString(deadPlayer.GetCurrentRoom()));

                // Reset the game
                StartCoroutine(ResetAfterGameOver());
                break;

            // If the state is level over
            case GameState.LevelOver:
                // Add the player data to the data tracker
                DataTracker.Instance.AddNewPlayerData(ref m_currentPlayerData);

                // Create a new player data
                m_currentPlayerData = new PlayerData();
                break;
        }
    }

    /// <summary>
    /// Reset the game after the player dies
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetAfterGameOver()
    {
        // Wait for 5 seconds
        yield return new WaitForSeconds(5f);

        // Set the state to reset
        ChangeState(GameState.Reset);
    }

    /// <summary>
    /// Update the amount of lives the player has
    /// </summary>
    /// <param name="_lives">Lives value</param>
    public void UpdateCurrentLevelLives(int _lives)
    {
        m_currentPlayerData.m_currentPlayerLives = _lives;
    }

    /// <summary>
    /// Update the score value
    /// </summary>
    /// <param name="_score">Score value</param>
    public void UpdateCurrentLevelScore(int _score)
    {
        m_currentPlayerData.m_score = _score;
    }

    /// <summary>
    /// Allow other classes access to the current game state
    /// </summary>
    /// <returns>Returns the current game state</returns>
    public GameState GetGameState()
    {
        return m_gameState;
    }

    /// <summary>
    /// Allows other classes to see if the game is in debug mode
    /// </summary>
    /// <returns>Returns true if the game is in debug mode but false if not</returns>
    public bool IsDebug()
    {
        return m_debugMode;
    }

    public void UpdateDataOnDatabase()
    {
        DataTracker.Instance.UpdateCurrentPlayerData(ref m_currentPlayerData);
        DataTracker.Instance.UpdatePlaythroughData();
    }
}

/// <summary>
/// Enum containing all of the game states
/// </summary>
public enum GameState
{
    Init,
    Play,
    Pause,
    GameOver,
    Reset,
    LevelOver,
}