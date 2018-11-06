using Firebase.Database;
using LevelGeneration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;

/// <summary>
/// Class to handle the data tracking over the playthrough of the demo
/// </summary>
public class DataTracker : Singleton<DataTracker>
{
    // Designer variables

    // File name of the exported data file
    [SerializeField]
    private string m_fileName = "TrackedData";

    [SerializeField]
    private string m_fileNameLevel = "TrackedLevelData";

    // Enable debug mode
    [SerializeField]
    private bool m_debugMode = true;

    // Debug export button
    [SerializeField]
    private KeyCode m_debugExportButton = KeyCode.Hash;

    [SerializeField]
    private TemplateHolder m_templateHolder;

    // Private variables

    // List of all tracked data
    private List<PlayerData> m_playerDataList = new List<PlayerData>();
    private List<DesignData> m_levelDataList = new List<DesignData>();
    private List<Difficulty> m_difficultyDataList = new List<Difficulty>();

    private Difficulty m_currentDifficulty;
    private Difficulty m_nextDifficulty;

    private PlaythroughData m_playthroughData;
    private PlaythroughData m_debugPlaythroughData;

    private Averages m_curEasyAverages;
    private Averages m_curMedAverages;
    private Averages m_curHardAverages;

    private Averages m_newEasyAverages;
    private Averages m_newMedAverages;
    private Averages m_newHardAverages;

    private PlayerData m_lastData;

    /// <summary>
    /// Initialise the data tracker object
    /// </summary>
    protected override void Awake()
    {
        // Use singleton class awake
        base.Awake();

        // Create a new playthrough data
        m_playthroughData = new PlaythroughData();

        // Create a new level data list
        m_playthroughData.m_levelData = new List<LevelData>();

        // Set up the first level data
        LevelData firstLevelData = new LevelData();
        firstLevelData.m_designData = new DesignData();
        firstLevelData.m_difficulty = m_currentDifficulty;
        firstLevelData.m_playerData = new PlayerData();

        // Create the behavioural data
        firstLevelData.m_playerData.m_behaviouralData = new BehaviouralData();
        firstLevelData.m_playerData.m_behaviouralData.m_difficultyScore = 0;

        // Create the death behavioural data
        firstLevelData.m_playerData.m_behaviouralData.m_deathBehaviouralAverages = new BehaviouralAverages();
        firstLevelData.m_playerData.m_behaviouralData.m_deathBehaviouralAverages.m_averageValue = 0;
        firstLevelData.m_playerData.m_behaviouralData.m_deathBehaviouralAverages.m_standardDeviationValue = 0;
        firstLevelData.m_playerData.m_behaviouralData.m_deathBehaviouralAverages.m_lowerBoundsValue = 0;
        firstLevelData.m_playerData.m_behaviouralData.m_deathBehaviouralAverages.m_upperBoundsValue = 0;

        // Create the score behavioural data
        firstLevelData.m_playerData.m_behaviouralData.m_scoreBehaviouralAverages = new BehaviouralAverages();
        firstLevelData.m_playerData.m_behaviouralData.m_scoreBehaviouralAverages.m_averageValue = 0;
        firstLevelData.m_playerData.m_behaviouralData.m_scoreBehaviouralAverages.m_standardDeviationValue = 0;
        firstLevelData.m_playerData.m_behaviouralData.m_scoreBehaviouralAverages.m_lowerBoundsValue = 0;
        firstLevelData.m_playerData.m_behaviouralData.m_scoreBehaviouralAverages.m_upperBoundsValue = 0;

        // Create the time behavioural data
        firstLevelData.m_playerData.m_behaviouralData.m_timeBehaviouralAverages = new BehaviouralAverages();
        firstLevelData.m_playerData.m_behaviouralData.m_timeBehaviouralAverages.m_averageValue = 0;
        firstLevelData.m_playerData.m_behaviouralData.m_timeBehaviouralAverages.m_standardDeviationValue = 0;
        firstLevelData.m_playerData.m_behaviouralData.m_timeBehaviouralAverages.m_lowerBoundsValue = 0;
        firstLevelData.m_playerData.m_behaviouralData.m_timeBehaviouralAverages.m_upperBoundsValue = 0;

        // Add the first level data to the level data list
        m_playthroughData.m_levelData.Add(firstLevelData);

        // Set the playthrough ID
        m_playthroughData.m_id = DatabaseManager.Instance.GetPlaythroughId();

        // Set the level ID
        m_playthroughData.m_levelId = 0;

        // Set the current difficulty for the first level to easy
        m_currentDifficulty = Difficulty.Easy;

        // If the database manager has a start difficulty override
        if (DatabaseManager.Instance.IsStartDifficultyOverride())
        {
            // Set the current difficulty to the start difficulty
            m_currentDifficulty = DatabaseManager.Instance.StartDifficulty();
        }

        // Update the current levels difficulty
        m_playthroughData.m_levelData[m_playthroughData.m_levelId].m_difficulty = m_currentDifficulty;

        // Create a new current easy averages
        m_curEasyAverages = new Averages();
        m_curEasyAverages.m_scoreAverages = new DataAverages();
        m_curEasyAverages.m_deathsAverages = new DataAverages();
        m_curEasyAverages.m_timeAverages = new DataAverages();
        m_curEasyAverages = DatabaseManager.Instance.AccessInitEasyAverages();

        // Create a new current medium averages
        m_curMedAverages = new Averages();
        m_curMedAverages.m_scoreAverages = new DataAverages();
        m_curMedAverages.m_deathsAverages = new DataAverages();
        m_curMedAverages.m_timeAverages = new DataAverages();
        m_curMedAverages = DatabaseManager.Instance.AccessInitMediumAverages();

        // Create a new current hard averages
        m_curHardAverages = new Averages();
        m_curHardAverages.m_scoreAverages = new DataAverages();
        m_curHardAverages.m_deathsAverages = new DataAverages();
        m_curHardAverages.m_timeAverages = new DataAverages();
        m_curHardAverages = DatabaseManager.Instance.AccessInitHardAverages();

        // Create a new easy averages
        m_newEasyAverages = new Averages();
        m_newEasyAverages.m_scoreAverages = new DataAverages();
        m_newEasyAverages.m_deathsAverages = new DataAverages();
        m_newEasyAverages.m_timeAverages = new DataAverages();

        // Create a new medium averages
        m_newMedAverages = new Averages();
        m_newMedAverages.m_scoreAverages = new DataAverages();
        m_newMedAverages.m_deathsAverages = new DataAverages();
        m_newMedAverages.m_timeAverages = new DataAverages();

        // Create a new hard averages
        m_newHardAverages = new Averages();
        m_newHardAverages.m_scoreAverages = new DataAverages();
        m_newHardAverages.m_deathsAverages = new DataAverages();
        m_newHardAverages.m_timeAverages = new DataAverages();
    }

    /// <summary>
    /// Update function that can be used for debugging
    /// </summary>
    private void Update()
    {
        // Check if the current scene is the main menu
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            // Increase the playthrough count
            IncreasePlaythroughCount();

            // Destroy the Data Tracker object
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Adds new data to the tracked data list
    /// </summary>
    /// <param name="_data">Current levels player data</param>
    public void AddNewPlayerData(ref PlayerData _data)
    {
        // Add new data to the tracked data list
        m_playthroughData.m_levelData[m_playthroughData.m_levelId].m_playerData = _data;

        // Compare the player data against the averages
        ComparePlayerData(_data);
    }

    /// <summary>
    /// Update the current player data in the current playthrough data
    /// </summary>
    /// <param name="_data"></param>
    public void UpdateCurrentPlayerData(ref PlayerData _data)
    {
        // Override the tracked data list player data
        m_playthroughData.m_levelData[m_playthroughData.m_levelId].m_playerData = _data;
    }

    /// <summary>
    /// Compares the current player data against the averages
    /// </summary>
    /// <param name="_playerData">Current levels player data</param>
    public void ComparePlayerData(PlayerData _playerData)
    {
        // Set a difficulty score of 0
        float difficultyScore = 0;

        // Compare death data
        // Set the player deaths count
        float playerDeathCount = _playerData.m_currentPlayerDeaths;

        // Get the death averages based on the current difficulty
        float deathAverage = 0;
        if (m_currentDifficulty == Difficulty.Easy)
        {
            deathAverage = m_curEasyAverages.m_deathsAverages.m_average;
        }
        else if (m_currentDifficulty == Difficulty.Medium)
        {
            deathAverage = m_curMedAverages.m_deathsAverages.m_average;
        }
        else
        {
            deathAverage = m_curHardAverages.m_deathsAverages.m_average;
        }

        // Get the death standard deviation based on the current difficulty
        float deathStandardDeviation = 0;
        if (m_currentDifficulty == Difficulty.Easy)
        {
            deathStandardDeviation = m_curEasyAverages.m_deathsAverages.m_standardDeviation;
        }
        else if (m_currentDifficulty == Difficulty.Medium)
        {
            deathStandardDeviation = m_curMedAverages.m_deathsAverages.m_standardDeviation;
        }
        else
        {
            deathStandardDeviation = m_curHardAverages.m_deathsAverages.m_standardDeviation;
        }

        // Calculate the lower and upper bounds for the death scores
        float deathLowerBounds = deathAverage - deathStandardDeviation;
        float deathUpperBounds = deathAverage + deathStandardDeviation;

        // Check if the death count is within the two bounds
        if (playerDeathCount >= deathLowerBounds && playerDeathCount <= deathUpperBounds)
        {
            // Difficulty score doesnt increase or decrease
            Debug.Log("Comparing player deaths, difficulty should stay the same");
        }

        // Check if the death count is below the lower bounds
        if (playerDeathCount < deathLowerBounds)
        {
            // Difficulty score increases by 2
            Debug.Log("Comparing player deaths, difficulty should increase");
            difficultyScore += 2;
        }

        // Check if the death count is greater than the upper bounds
        if (playerDeathCount > deathUpperBounds)
        {
            // Difficulty score decreases by 2
            Debug.Log("Comparing player deaths, difficulty should decrease");
            difficultyScore -= 2;
        }

        // Compare score data
        // Set the player score
        float playerScore = _playerData.m_score;

        // Get the score average based on the current difficulty
        float scoreAverage = 0;
        if (m_currentDifficulty == Difficulty.Easy)
        {
            scoreAverage = m_curEasyAverages.m_scoreAverages.m_average;
        }
        else if (m_currentDifficulty == Difficulty.Medium)
        {
            scoreAverage = m_curMedAverages.m_scoreAverages.m_average;
        }
        else
        {
            scoreAverage = m_curHardAverages.m_scoreAverages.m_average;
        }

        // Get the score standard deviation based on the current difficulty
        float scoreStandardDeviation = 0;
        if (m_currentDifficulty == Difficulty.Easy)
        {
            scoreStandardDeviation = m_curEasyAverages.m_scoreAverages.m_standardDeviation;
        }
        else if (m_currentDifficulty == Difficulty.Medium)
        {
            scoreStandardDeviation = m_curMedAverages.m_scoreAverages.m_standardDeviation;
        }
        else
        {
            scoreStandardDeviation = m_curHardAverages.m_scoreAverages.m_standardDeviation;
        }

        // Calculate the scores lower and upper bounds
        float scoreLowerBounds = scoreAverage - scoreStandardDeviation;
        float scoreUpperBounds = scoreAverage + scoreStandardDeviation;

        // Check if the player score is within the two bounds
        if (playerScore >= scoreLowerBounds && playerScore <= scoreUpperBounds)
        {
            // Difficulty score doesnt increase or decrease
            Debug.Log("Comparing player score, difficulty should stay the same");
        }
        if (playerScore < scoreLowerBounds)
        {
            // Difficulty score is decreased by 2
            Debug.Log("Comparing player score, difficulty should decrease");
            difficultyScore -= 2;
        }
        // Check if the player score is greater than the upper bounds
        if (playerScore > scoreUpperBounds)
        {
            // Difficulty score is increased by 2
            Debug.Log("Comparing player score, difficulty should increase");
            difficultyScore += 2;
        }

        // Compare time data
        float playerTime = _playerData.m_timeTakenToCompleteLevel;

        // Get the time average based on the current difficulty
        float timeAverage = 0;
        if (m_currentDifficulty == Difficulty.Easy)
        {
            timeAverage = m_curEasyAverages.m_timeAverages.m_average;
        }
        else if (m_currentDifficulty == Difficulty.Medium)
        {
            timeAverage = m_curMedAverages.m_timeAverages.m_average;
        }
        else
        {
            timeAverage = m_curHardAverages.m_timeAverages.m_average;
        }

        // Get the time standard deviation based on the current difficulty
        float timeStandardDeviation = 0;
        if (m_currentDifficulty == Difficulty.Easy)
        {
            timeStandardDeviation = m_curEasyAverages.m_timeAverages.m_standardDeviation;
        }
        else if (m_currentDifficulty == Difficulty.Medium)
        {
            timeStandardDeviation = m_curMedAverages.m_timeAverages.m_standardDeviation;
        }
        else
        {
            timeStandardDeviation = m_curHardAverages.m_timeAverages.m_standardDeviation;
        }

        // Calculate the time upper and lower bounds
        float timeLowerBounds = timeAverage - timeStandardDeviation;
        float timeUpperBounds = timeAverage + timeStandardDeviation;

        // Check if the player time is within the two bounds
        if (playerTime >= timeLowerBounds && playerTime <= timeUpperBounds)
        {
            // Difficulty score doesnt increase or decrease
            Debug.Log("Comparing player time, difficulty should stay the same");
        }

        // Check if the player time is less than the lower bounds
        if (playerTime < timeLowerBounds)
        {
            // Difficulty score is increased by 2
            Debug.Log("Comparing player time, difficulty should increase");
            difficultyScore += 2;
        }

        // Check if the player time is greater than the upper bounds
        // Take into consideration the score as well as the score factors into time
        if (playerTime > timeUpperBounds && playerScore < scoreUpperBounds)
        {
            // Difficulty score is decreased by 2
            Debug.Log("Comparing player time, difficulty should decrease");
            difficultyScore -= 2;
        }
        else if (playerTime > timeUpperBounds && playerScore >= scoreUpperBounds)
        {
            // Difficulty score is increased by 2
            Debug.Log("Comparing player time, difficulty should increase");
            difficultyScore += 2;
        }

        // Check difficulty score
        // If greater than 2 increase difficulty and not the hardest difficulty
        if (difficultyScore > 2 && m_currentDifficulty != Difficulty.Hard)
        {
            // Increase the next difficulty
            int value = (int)m_currentDifficulty + 1;
            m_nextDifficulty = (Difficulty)value;
        }

        // If less than -2 decrease difficulty and not the easiest difficulty
        if (difficultyScore < -2 && m_currentDifficulty != Difficulty.Easy)
        {
            // Decrease the next difficulty
            int value = (int)m_currentDifficulty - 1;
            m_nextDifficulty = (Difficulty)value;
        }

        // Update the behavioural data
        m_playthroughData.m_levelData[m_playthroughData.m_levelId].m_playerData.m_behaviouralData = new BehaviouralData();
        m_playthroughData.m_levelData[m_playthroughData.m_levelId].m_playerData.m_behaviouralData.m_difficultyScore =
            difficultyScore;

        // Update the death behavioural data
        m_playthroughData.m_levelData[m_playthroughData.m_levelId].m_playerData.m_behaviouralData.m_deathBehaviouralAverages = new BehaviouralAverages();
        m_playthroughData.m_levelData[m_playthroughData.m_levelId].m_playerData.m_behaviouralData
            .m_deathBehaviouralAverages.m_averageValue = deathAverage;
        m_playthroughData.m_levelData[m_playthroughData.m_levelId].m_playerData.m_behaviouralData
            .m_deathBehaviouralAverages.m_standardDeviationValue = deathStandardDeviation;
        m_playthroughData.m_levelData[m_playthroughData.m_levelId].m_playerData.m_behaviouralData
            .m_deathBehaviouralAverages.m_lowerBoundsValue = deathLowerBounds;
        m_playthroughData.m_levelData[m_playthroughData.m_levelId].m_playerData.m_behaviouralData
            .m_deathBehaviouralAverages.m_upperBoundsValue = deathUpperBounds;

        // Update the score behavioural data
        m_playthroughData.m_levelData[m_playthroughData.m_levelId].m_playerData.m_behaviouralData.m_scoreBehaviouralAverages = new BehaviouralAverages();
        m_playthroughData.m_levelData[m_playthroughData.m_levelId].m_playerData.m_behaviouralData
            .m_scoreBehaviouralAverages.m_averageValue = scoreAverage;
        m_playthroughData.m_levelData[m_playthroughData.m_levelId].m_playerData.m_behaviouralData
            .m_scoreBehaviouralAverages.m_standardDeviationValue = scoreStandardDeviation;
        m_playthroughData.m_levelData[m_playthroughData.m_levelId].m_playerData.m_behaviouralData
            .m_scoreBehaviouralAverages.m_lowerBoundsValue = scoreLowerBounds;
        m_playthroughData.m_levelData[m_playthroughData.m_levelId].m_playerData.m_behaviouralData
            .m_scoreBehaviouralAverages.m_upperBoundsValue = scoreUpperBounds;

        // Update the time behavioural data
        m_playthroughData.m_levelData[m_playthroughData.m_levelId].m_playerData.m_behaviouralData.m_timeBehaviouralAverages = new BehaviouralAverages();
        m_playthroughData.m_levelData[m_playthroughData.m_levelId].m_playerData.m_behaviouralData
            .m_timeBehaviouralAverages.m_averageValue = timeAverage;
        m_playthroughData.m_levelData[m_playthroughData.m_levelId].m_playerData.m_behaviouralData
            .m_timeBehaviouralAverages.m_standardDeviationValue = timeStandardDeviation;
        m_playthroughData.m_levelData[m_playthroughData.m_levelId].m_playerData.m_behaviouralData
            .m_timeBehaviouralAverages.m_lowerBoundsValue = timeLowerBounds;
        m_playthroughData.m_levelData[m_playthroughData.m_levelId].m_playerData.m_behaviouralData
            .m_timeBehaviouralAverages.m_upperBoundsValue = timeUpperBounds;
    }

    /// <summary>
    /// Add a new level data to the playthrough data
    /// </summary>
    /// <param name="_levelData">Current level data</param>
    public void AddNewLevelData(ref DesignData _levelData)
    {
        m_playthroughData.m_levelData[m_playthroughData.m_levelId].m_designData = _levelData;
    }

    /// <summary>
    /// Create and set up a new level data struct,
    /// send old data to the database
    /// </summary>
    /// <param name="levelCount">sends out the new level count</param>
    /// <param name="wait">Force the game to wait for the data to be finished being sent</param>
    public void SetUpNextLevelDataAndSendToDatabase(out int levelCount, bool wait)
    {
        // Send playthrough data to the database
        UpdatePlaythroughData(wait);

        // Increase the level count
        m_playthroughData.m_levelId++;
        levelCount = m_playthroughData.m_levelId;
    }

    /// <summary>
    /// Set up the new data for the next level
    /// Add the new data to th playthrough data
    /// Recalculate the averages and standard deviations
    /// </summary>
    private void SetUpNewDataAndRecalculateAverages()
    {
        // Create new level data
        LevelData newLevelData = new LevelData();
        newLevelData.m_designData = new DesignData();
        newLevelData.m_difficulty = m_currentDifficulty;
        newLevelData.m_playerData = new PlayerData();

        // Create new behavioural data
        newLevelData.m_playerData.m_behaviouralData = new BehaviouralData();
        newLevelData.m_playerData.m_behaviouralData.m_difficultyScore = 0;

        // Create new death behavioural data
        newLevelData.m_playerData.m_behaviouralData.m_deathBehaviouralAverages = new BehaviouralAverages();
        newLevelData.m_playerData.m_behaviouralData.m_deathBehaviouralAverages.m_averageValue = 0;
        newLevelData.m_playerData.m_behaviouralData.m_deathBehaviouralAverages.m_standardDeviationValue = 0;
        newLevelData.m_playerData.m_behaviouralData.m_deathBehaviouralAverages.m_lowerBoundsValue = 0;
        newLevelData.m_playerData.m_behaviouralData.m_deathBehaviouralAverages.m_upperBoundsValue = 0;

        // Create new score behavioural data
        newLevelData.m_playerData.m_behaviouralData.m_scoreBehaviouralAverages = new BehaviouralAverages();
        newLevelData.m_playerData.m_behaviouralData.m_scoreBehaviouralAverages.m_averageValue = 0;
        newLevelData.m_playerData.m_behaviouralData.m_scoreBehaviouralAverages.m_standardDeviationValue = 0;
        newLevelData.m_playerData.m_behaviouralData.m_scoreBehaviouralAverages.m_lowerBoundsValue = 0;
        newLevelData.m_playerData.m_behaviouralData.m_scoreBehaviouralAverages.m_upperBoundsValue = 0;

        // Create new time behavioural data
        newLevelData.m_playerData.m_behaviouralData.m_timeBehaviouralAverages = new BehaviouralAverages();
        newLevelData.m_playerData.m_behaviouralData.m_timeBehaviouralAverages.m_averageValue = 0;
        newLevelData.m_playerData.m_behaviouralData.m_timeBehaviouralAverages.m_standardDeviationValue = 0;
        newLevelData.m_playerData.m_behaviouralData.m_timeBehaviouralAverages.m_lowerBoundsValue = 0;
        newLevelData.m_playerData.m_behaviouralData.m_timeBehaviouralAverages.m_upperBoundsValue = 0;

        // Add new level data to playthrough data's level data list
        m_playthroughData.m_levelData.Add(newLevelData);

        // Get all existing data on profile and recalculate the averages
        GetExistingPlaythroughs();
    }

    /// <summary>
    /// Update the playthrough data onto the database
    /// </summary>
    /// <param name="wait">Force the game to wait for the data to be finished being sent</param>
    public void UpdatePlaythroughData(bool wait = false)
    {
        // Check if the database is ready
        if (DatabaseManager.Instance.IsDatabaseReady())
        {
            // Convert the playthrough data to JSON
            string json = JsonUtility.ToJson(m_playthroughData);

            // Set the playthrough path
            string path = "Playthrough " + m_playthroughData.m_id;

            // If waiting, use the wait data push coroutine
            if (wait)
            {
                StartCoroutine(WatingDataPush(json, path, wait));
            }
            else
            {
                // Send playthrough data to database
                Task task = DatabaseManager.Instance.GetDatabaseReference()
                    .Child(DatabaseManager.Instance.AccessId())
                    .Child("Playthroughs")
                    .Child(path)
                    .SetRawJsonValueAsync(json);
            }
        }
    }

    /// <summary>
    /// Force the game to wait until the data has been sent to the database
    /// </summary>
    /// <param name="json">JSON version of the data being sent</param>
    /// <param name="path">Database path</param>
    /// <param name="wait">Does the game need to wait</param>
    /// <returns>Null</returns>
    private IEnumerator WatingDataPush(string json, string path, bool wait)
    {
        // Send playthrough data to database
        Task task = DatabaseManager.Instance.GetDatabaseReference()
            .Child(DatabaseManager.Instance.AccessId())
            .Child("Playthroughs")
            .Child(path)
            .SetRawJsonValueAsync(json);
        // If waiting and the task is not complete
        while (wait && !task.IsCompleted)
        {
            // Return null
            Debug.Log("Waiting");
            yield return null;
        }
        // If waiting and the task is complete
        if (wait && task.IsCompleted)
        {
            // Get the level
            Debug.Log("Sent");
            int levelcount = GetLevelId();
            // Set up the new data and recalculate the averages
            SetUpNewDataAndRecalculateAverages();
            // Call the Level Finished function wtih next level count
            GameManager.Instance.LevelFinished(levelcount++);
        }
    }

    /// <summary>
    /// Called when the object is destroyed and used to
    /// increase the playthrough count
    /// </summary>
    private void OnApplicationQuit()
    {
        // Increase the playthrough count
        IncreasePlaythroughCount();
    }

    /// <summary>
    /// Increase the playthrough count in game and database
    /// </summary>
    private void IncreasePlaythroughCount()
    {
        // Check if there is a database manager
        if (DatabaseManager.Instance)
        {
            // Check if the database is ready
            if (DatabaseManager.Instance.IsDatabaseReady())
            {
                // Increase the playthrough ID on the database manager object
                DatabaseManager.Instance.IncreasePlaythroughId();

                // Send the new playthrough ID to the database
                string path = "PlaythroughCount";
                DatabaseManager.Instance.GetDatabaseReference().Child(DatabaseManager.Instance.AccessId())
                    .Child(path)
                    .SetValueAsync(DatabaseManager.Instance.GetPlaythroughId());
            }
        }
        else // If no database manager display error message
        {
            Debug.LogError("No DatabaseManager");
        }
    }

    /// <summary>
    /// Decide the next level's difficulty
    /// </summary>
    public void DecideNextDifficulty()
    {
        // If randomly selecting difficulty, pick a random number
        int rand = UnityEngine.Random.Range(0, (int)Difficulty.Count);

        // Set the current difficulty to the Difficulty enum version of that number
        m_currentDifficulty = (Difficulty)rand;

        // If not randomly selecting difficulty set current difficulty to the next difficulty
        if (!DatabaseManager.Instance.IsDifficultyRandomised())
        {
            m_currentDifficulty = m_nextDifficulty;
        }

        // Update the difficulty in the current level data
        m_playthroughData.m_levelData[m_playthroughData.m_levelId].m_difficulty = m_currentDifficulty;
    }

    /// <summary>
    /// Get the current difficulty
    /// </summary>
    /// <returns>Current difficulty</returns>
    public Difficulty GetDifficulty()
    {
        return m_currentDifficulty;
    }

    /// <summary>
    /// Get the current playthrough ID
    /// </summary>
    /// <returns>Current playthrough ID</returns>
    public int GetLevelId()
    {
        return m_playthroughData.m_levelId;
    }

    /// <summary>
    /// Gets all existing playthrough data of the user
    /// </summary>
    private void GetExistingPlaythroughs()
    {
        // Create a list of playthrough data
        List<PlaythroughData> existingPlaythroughData = new List<PlaythroughData>();

        // Create a easy overall player data struct
        OverallPlayerData easyOverallPlayerData = new OverallPlayerData();
        easyOverallPlayerData.m_deaths = new List<float>();
        easyOverallPlayerData.m_scores = new List<float>();
        easyOverallPlayerData.m_times = new List<float>();

        // Create a medium overall player data struct
        OverallPlayerData mediumOverallPlayerData = new OverallPlayerData();
        mediumOverallPlayerData.m_deaths = new List<float>();
        mediumOverallPlayerData.m_scores = new List<float>();
        mediumOverallPlayerData.m_times = new List<float>();

        // Create a hard overall player data struct
        OverallPlayerData hardOverallPlayerData = new OverallPlayerData();
        hardOverallPlayerData.m_deaths = new List<float>();
        hardOverallPlayerData.m_scores = new List<float>();
        hardOverallPlayerData.m_times = new List<float>();

        // Check if the database is ready
        if (DatabaseManager.Instance.IsDatabaseReady())
        {
            // Get all the playthrough data
            DatabaseManager.Instance.GetDatabaseReference().Child(DatabaseManager.Instance.AccessId()).
                Child("Playthroughs").GetValueAsync().ContinueWith(countTask =>
            {
                // Check for any faults
                if (countTask.IsFaulted)
                {
                    // Handle the error...
                    Debug.LogError("Failed to get Playthroughs");
                }
                // If no faults
                else if (countTask.IsCompleted)
                {
                    // Get the data snapshot from the task result
                    DataSnapshot snapshot = countTask.Result;

                    // Calculate level id
                    int id = m_playthroughData.m_levelId - 1;

                    // If this is the first playthrough and the level id is 0
                    if (DatabaseManager.Instance.GetPlaythroughId() == 0 && id == 0)
                    {
                        // Check if the difficulty is easy
                        if (m_playthroughData.m_levelData[0].m_difficulty == Difficulty.Easy)
                        {
                            // Set the easy overall player data
                            easyOverallPlayerData.m_deaths.Add(m_playthroughData.m_levelData[0]
                                .m_playerData.m_currentPlayerDeaths);
                            easyOverallPlayerData.m_scores.Add(m_playthroughData.m_levelData[0]
                                .m_playerData.m_score);
                            easyOverallPlayerData.m_times.Add(m_playthroughData.m_levelData[0]
                                .m_playerData.m_timeTakenToCompleteLevel);
                        }
                        // Check if the difficulty is medium
                        else if (m_playthroughData.m_levelData[0].m_difficulty == Difficulty.Medium)
                        {
                            // Set the medium overall player data
                            mediumOverallPlayerData.m_deaths.Add(m_playthroughData.m_levelData[0]
                                .m_playerData.m_currentPlayerDeaths);
                            mediumOverallPlayerData.m_scores.Add(m_playthroughData.m_levelData[0]
                                .m_playerData.m_score);
                            mediumOverallPlayerData.m_times.Add(m_playthroughData.m_levelData[0]
                                .m_playerData.m_timeTakenToCompleteLevel);
                        }
                        // Check if the difficulty is hard
                        else if (m_playthroughData.m_levelData[0].m_difficulty == Difficulty.Hard)
                        {
                            // Set the hard overall player data
                            hardOverallPlayerData.m_deaths.Add(m_playthroughData.m_levelData[0]
                                .m_playerData.m_currentPlayerDeaths);
                            hardOverallPlayerData.m_scores.Add(m_playthroughData.m_levelData[0]
                                .m_playerData.m_score);
                            hardOverallPlayerData.m_times.Add(m_playthroughData.m_levelData[0]
                                .m_playerData.m_timeTakenToCompleteLevel);
                        }
                        // If it is none of the defined difficulties then display error message
                        else
                        {
                            Debug.LogError("Invalid Difficulty");
                        }
                    }
                    else // Else loop through all of the playthrough children
                    {
                        for (int index = 0; index <= DatabaseManager.Instance.GetPlaythroughId(); index++)
                        {
                            // Set the path
                            string path = "Playthrough " + index;
                            Debug.Log(path);

                            // Check if the snapshot has a child at the path
                            if (snapshot.HasChild(path))
                            {
                                Debug.Log("Has Path");
                                // Get the raw JSON data from that child
                                string rawJson = snapshot.Child(path).GetRawJsonValue();

                                // Convert the JSON data into the playthrough data
                                PlaythroughData playthroughData = JsonUtility.FromJson<PlaythroughData>(rawJson);

                                // Loop through all of the level data
                                for (int levelIndex = 0; levelIndex < playthroughData.m_levelData.Count; levelIndex++)
                                {
                                    // Check if the level's difficulty is easy and add the data
                                    // to the easy overall player data
                                    if (playthroughData.m_levelData[levelIndex].m_difficulty == Difficulty.Easy)
                                    {
                                        easyOverallPlayerData.m_deaths.Add(playthroughData.m_levelData[levelIndex]
                                            .m_playerData.m_currentPlayerDeaths);
                                        easyOverallPlayerData.m_scores.Add(playthroughData.m_levelData[levelIndex]
                                            .m_playerData.m_score);
                                        easyOverallPlayerData.m_times.Add(playthroughData.m_levelData[levelIndex]
                                            .m_playerData.m_timeTakenToCompleteLevel);
                                    }

                                    // Check if the level's difficulty is medium and add the data
                                    // to the medium overall player data
                                    if (playthroughData.m_levelData[levelIndex].m_difficulty == Difficulty.Medium)
                                    {
                                        mediumOverallPlayerData.m_deaths.Add(playthroughData.m_levelData[levelIndex]
                                            .m_playerData.m_currentPlayerDeaths);
                                        mediumOverallPlayerData.m_scores.Add(playthroughData.m_levelData[levelIndex]
                                            .m_playerData.m_score);
                                        mediumOverallPlayerData.m_times.Add(playthroughData.m_levelData[levelIndex]
                                            .m_playerData.m_timeTakenToCompleteLevel);
                                    }

                                    // Check if the level's difficulty is hard and add the data
                                    // to the hard overall player data
                                    if (playthroughData.m_levelData[levelIndex].m_difficulty == Difficulty.Hard)
                                    {
                                        hardOverallPlayerData.m_deaths.Add(playthroughData.m_levelData[levelIndex]
                                            .m_playerData.m_currentPlayerDeaths);
                                        hardOverallPlayerData.m_scores.Add(playthroughData.m_levelData[levelIndex]
                                            .m_playerData.m_score);
                                        hardOverallPlayerData.m_times.Add(playthroughData.m_levelData[levelIndex]
                                            .m_playerData.m_timeTakenToCompleteLevel);
                                    }
                                }
                                // Assign the playthrough data to the debug playthrough data so that
                                // it can be seen in the inspector
                                m_debugPlaythroughData = playthroughData;
                            }
                            else // If there is no path then display error message
                            {
                                Debug.LogError("No Path");
                            }
                        }
                    }

                    // Recalculate the averages stats
                    RecalculateStats(ref easyOverallPlayerData, ref mediumOverallPlayerData, ref hardOverallPlayerData);

                    if (m_currentDifficulty == Difficulty.Easy)
                    {
                        // Convert the new easy averages stats to JSON
                        string rawEasyJson = JsonUtility.ToJson(m_newEasyAverages);

                        // Send the new averages to the database
                        DatabaseManager.Instance.GetDatabaseReference().Child(DatabaseManager.Instance.AccessId()).
                        Child("Averages").Child("Easy Averages").SetRawJsonValueAsync(rawEasyJson);
                    }

                    if (m_currentDifficulty == Difficulty.Medium)
                    {
                        // Convert the new medium averages stats to JSON
                        string rawMediumJson = JsonUtility.ToJson(m_newMedAverages);

                        // Send the new averages to the database
                        DatabaseManager.Instance.GetDatabaseReference().Child(DatabaseManager.Instance.AccessId())
                            .Child("Averages").Child("Medium Averages").SetRawJsonValueAsync(rawMediumJson);
                    }

                    if (m_currentDifficulty == Difficulty.Hard)
                    {
                        // Convert the new hard averages stats to JSON
                        string rawHardJson = JsonUtility.ToJson(m_newHardAverages);

                        // Send the new averages to the database
                        DatabaseManager.Instance.GetDatabaseReference().Child(DatabaseManager.Instance.AccessId())
                            .Child("Averages").Child("Hard Averages").SetRawJsonValueAsync(rawHardJson);
                    }

                    // Update the current averages with the new averages
                    if (m_currentDifficulty == Difficulty.Easy)
                    {
                        m_curEasyAverages = m_newEasyAverages;
                    }

                    if (m_currentDifficulty == Difficulty.Medium)
                    {
                        m_curMedAverages = m_newMedAverages;
                    }

                    if (m_currentDifficulty == Difficulty.Hard)
                    {
                        m_curHardAverages = m_newHardAverages;
                    }
                }
            });
        }
    }

    /// <summary>
    /// Recalculate the stats used to decide the change in difficulty
    /// </summary>
    /// <param name="easyOverallPlayerData">Easy overall player data</param>
    /// <param name="mediumOverallPlayerData">Medium overall player data</param>
    /// <param name="hardOverallPlayerData">Hard overall player data</param>
    private void RecalculateStats(ref OverallPlayerData easyOverallPlayerData, ref OverallPlayerData mediumOverallPlayerData,
        ref OverallPlayerData hardOverallPlayerData)
    {
        // Recalculate the new easy averages
        // Calcalulate the death averages
        m_newEasyAverages.m_deathsAverages.m_average = easyOverallPlayerData.m_deaths.Average();
        // Check if the value is NaN or infinity and set to 0 if either are true
        if (float.IsInfinity(m_newEasyAverages.m_deathsAverages.m_average) || float.IsNaN(m_newEasyAverages.m_deathsAverages.m_average))
        {
            m_newEasyAverages.m_deathsAverages.m_average = 0;
        }

        // Calculate the score averages
        m_newEasyAverages.m_scoreAverages.m_average = easyOverallPlayerData.m_scores.Average();
        // Check if the value is NaN or infinity and set to 0 if either are true
        if (float.IsInfinity(m_newEasyAverages.m_scoreAverages.m_average) || float.IsNaN(m_newEasyAverages.m_scoreAverages.m_average))
        {
            m_newEasyAverages.m_scoreAverages.m_average = 0;
        }

        // Calculate the time averages
        m_newEasyAverages.m_timeAverages.m_average =
            easyOverallPlayerData.m_times.Average();
        // Check if the value is NaN or infinity and set to 0 if either are true
        if (float.IsInfinity(m_newEasyAverages.m_timeAverages.m_average) || float.IsNaN(m_newEasyAverages.m_timeAverages.m_average))
        {
            m_newEasyAverages.m_timeAverages.m_average = 0;
        }

        // Recalculate the new easy standard deviations
        // Calculate the death standard deviation
        m_newEasyAverages.m_deathsAverages.m_standardDeviation =
            StatisticsMath.CalculateStandardDeviation(easyOverallPlayerData.m_deaths.Count,
                m_newEasyAverages.m_deathsAverages.m_average, easyOverallPlayerData.m_deaths.ToArray());
        // Check if the value is NaN or infinity and set to 0 if either are true
        if (float.IsInfinity(m_newEasyAverages.m_deathsAverages.m_standardDeviation) || float.IsNaN(m_newEasyAverages.m_deathsAverages.m_standardDeviation))
        {
            m_newEasyAverages.m_deathsAverages.m_standardDeviation = 0;
        }

        // Calculate the score standard deviation
        m_newEasyAverages.m_scoreAverages.m_standardDeviation =
            StatisticsMath.CalculateStandardDeviation(easyOverallPlayerData.m_scores.Count,
                m_newEasyAverages.m_scoreAverages.m_average, easyOverallPlayerData.m_scores.ToArray());
        // Check if the value is NaN or infinity and set to 0 if either are true
        if (float.IsInfinity(m_newEasyAverages.m_scoreAverages.m_standardDeviation) || float.IsNaN(m_newEasyAverages.m_scoreAverages.m_standardDeviation))
        {
            m_newEasyAverages.m_scoreAverages.m_standardDeviation = 0;
        }

        // Calculate time standard deviation
        m_newEasyAverages.m_timeAverages.m_standardDeviation =
            StatisticsMath.CalculateStandardDeviation(easyOverallPlayerData.m_times.Count,
                m_newEasyAverages.m_timeAverages.m_average, easyOverallPlayerData.m_times.ToArray());
        // Check if the value is NaN or infinity and set to 0 if either are true
        if (float.IsInfinity(m_newEasyAverages.m_timeAverages.m_standardDeviation) || float.IsNaN(m_newEasyAverages.m_timeAverages.m_standardDeviation))
        {
            m_newEasyAverages.m_timeAverages.m_standardDeviation = 0;
        }

        // Check if any of medium data counts are 0
        if (mediumOverallPlayerData.m_deaths.Count == 0 && mediumOverallPlayerData.m_scores.Count == 0
                                                        && mediumOverallPlayerData.m_times.Count == 0)
        {
            // Use the current medium averages
            Debug.Log("No Deaths, Score, Time in medium");
            m_newMedAverages = m_curMedAverages;
        }
        else // Else
        {
            // Calculate the death averages
            m_newMedAverages.m_deathsAverages.m_average =
                mediumOverallPlayerData.m_deaths.Average();
            // Check if the value is NaN or infinity and set to 0 if either are true
            if (float.IsInfinity(m_newMedAverages.m_deathsAverages.m_average) || float.IsNaN(m_newMedAverages.m_deathsAverages.m_average))
            {
                m_newMedAverages.m_deathsAverages.m_average = 0;
            }

            // Calculate the score averages
            m_newMedAverages.m_scoreAverages.m_average =
                mediumOverallPlayerData.m_scores.Average();
            // Check if the value is NaN or infinity and set to 0 if either are true
            if (float.IsInfinity(m_newMedAverages.m_scoreAverages.m_average) || float.IsNaN(m_newMedAverages.m_scoreAverages.m_average))
            {
                m_newMedAverages.m_scoreAverages.m_average = 0;
            }

            // Calculate the time averages
            m_newMedAverages.m_timeAverages.m_average =
                mediumOverallPlayerData.m_times.Average();
            // Check if the value is NaN or infinity and set to 0 if either are true
            if (float.IsInfinity(m_newMedAverages.m_timeAverages.m_average) || float.IsNaN(m_newMedAverages.m_timeAverages.m_average))
            {
                m_newMedAverages.m_timeAverages.m_average = 0;
            }

            // Calculate the death standard deviation
            m_newMedAverages.m_deathsAverages.m_standardDeviation =
                StatisticsMath.CalculateStandardDeviation(mediumOverallPlayerData.m_deaths.Count,
                    m_newMedAverages.m_deathsAverages.m_average, mediumOverallPlayerData.m_deaths.ToArray());
            // Check if the value is NaN or infinity and set to 0 if either are true
            if (float.IsInfinity(m_newMedAverages.m_deathsAverages.m_standardDeviation) ||
                float.IsNaN(m_newMedAverages.m_deathsAverages.m_standardDeviation))
            {
                m_newMedAverages.m_deathsAverages.m_standardDeviation = 0;
            }

            // Calculate the score standard deviation
            m_newMedAverages.m_scoreAverages.m_standardDeviation =
                StatisticsMath.CalculateStandardDeviation(mediumOverallPlayerData.m_scores.Count,
                    m_newMedAverages.m_scoreAverages.m_average, mediumOverallPlayerData.m_scores.ToArray());
            // Check if the value is NaN or infinity and set to 0 if either are true
            if (float.IsInfinity(m_newMedAverages.m_scoreAverages.m_standardDeviation) ||
                float.IsNaN(m_newMedAverages.m_scoreAverages.m_standardDeviation))
            {
                m_newMedAverages.m_scoreAverages.m_standardDeviation = 0;
            }

            // Calculate the time standard deviation
            m_newMedAverages.m_timeAverages.m_standardDeviation =
                StatisticsMath.CalculateStandardDeviation(mediumOverallPlayerData.m_times.Count,
                    m_newMedAverages.m_timeAverages.m_average, mediumOverallPlayerData.m_times.ToArray());
            // Check if the value is NaN or infinity and set to 0 if either are true
            if (float.IsInfinity(m_newMedAverages.m_timeAverages.m_standardDeviation) ||
                float.IsNaN(m_newMedAverages.m_timeAverages.m_standardDeviation))
            {
                m_newMedAverages.m_timeAverages.m_standardDeviation = 0;
            }
        }

        // Check if the hard player data counts are 0
        if (hardOverallPlayerData.m_deaths.Count == 0 && hardOverallPlayerData.m_scores.Count == 0
                                                        && hardOverallPlayerData.m_times.Count == 0)
        {
            // Use the current averages
            Debug.Log("No Deaths, Score, Time in hard");
            m_newHardAverages = m_curHardAverages;
        }
        else // Else
        {
            // Calculate the death average
            m_newHardAverages.m_deathsAverages.m_average =
                hardOverallPlayerData.m_deaths.Average();
            // Check if the value is NaN or infinity and set to 0 if either are true
            if (float.IsInfinity(m_newHardAverages.m_deathsAverages.m_average) ||
                float.IsNaN(m_newHardAverages.m_deathsAverages.m_average))
            {
                m_newHardAverages.m_deathsAverages.m_average = 0;
            }

            // Calculate the score average
            m_newHardAverages.m_scoreAverages.m_average =
                hardOverallPlayerData.m_scores.Average();
            // Check if the value is NaN or infinity and set to 0 if either are true
            if (float.IsInfinity(m_newHardAverages.m_scoreAverages.m_average) ||
                float.IsNaN(m_newHardAverages.m_scoreAverages.m_average))
            {
                m_newHardAverages.m_scoreAverages.m_average = 0;
            }

            // Calculate the time average
            m_newHardAverages.m_timeAverages.m_average =
                hardOverallPlayerData.m_times.Average();
            // Check if the value is NaN or infinity and set to 0 if either are true
            if (float.IsInfinity(m_newHardAverages.m_timeAverages.m_average) ||
                float.IsNaN(m_newHardAverages.m_timeAverages.m_average))
            {
                m_newHardAverages.m_timeAverages.m_average = 0;
            }

            // Recalculate the new hard standard deviations
            // Calculate the death standard deviation
            m_newHardAverages.m_deathsAverages.m_standardDeviation =
                StatisticsMath.CalculateStandardDeviation(hardOverallPlayerData.m_deaths.Count,
                    m_newHardAverages.m_deathsAverages.m_average, hardOverallPlayerData.m_deaths.ToArray());
            // Check if the value is NaN or infinity and set to 0 if either are true
            if (float.IsInfinity(m_newHardAverages.m_deathsAverages.m_standardDeviation) ||
                float.IsNaN(m_newHardAverages.m_deathsAverages.m_standardDeviation))
            {
                m_newHardAverages.m_deathsAverages.m_standardDeviation = 0;
            }

            // Calculate the score standard deviation
            m_newHardAverages.m_scoreAverages.m_standardDeviation =
                StatisticsMath.CalculateStandardDeviation(hardOverallPlayerData.m_scores.Count,
                    m_newHardAverages.m_scoreAverages.m_average, hardOverallPlayerData.m_scores.ToArray());
            // Check if the value is NaN or infinity and set to 0 if either are true
            if (float.IsInfinity(m_newHardAverages.m_scoreAverages.m_standardDeviation) ||
                float.IsNaN(m_newHardAverages.m_scoreAverages.m_standardDeviation))
            {
                m_newHardAverages.m_scoreAverages.m_standardDeviation = 0;
            }

            // Calculate the time standard deviation
            m_newHardAverages.m_timeAverages.m_standardDeviation =
                StatisticsMath.CalculateStandardDeviation(hardOverallPlayerData.m_times.Count,
                    m_newHardAverages.m_timeAverages.m_average, hardOverallPlayerData.m_times.ToArray());
            // Check if the value is NaN or infinity and set to 0 if either are true
            if (float.IsInfinity(m_newHardAverages.m_timeAverages.m_standardDeviation) ||
                float.IsNaN(m_newHardAverages.m_timeAverages.m_standardDeviation))
            {
                m_newHardAverages.m_timeAverages.m_standardDeviation = 0;
            }
        }
    }
}

/// <summary>
/// Struct to hold all of the tracked player data
/// </summary>
[System.Serializable]
public struct PlayerData
{
    public int m_currentPlayerLives;
    public int m_currentPlayerDeaths;
    public float m_timeTakenToCompleteLevel;
    public int m_enemiesKilled;
    public int m_score;
    public List<SimpleRoomData> m_deathRoomData;
    public BehaviouralData m_behaviouralData;
}

/// <summary>
/// Struct to hold all of the level's design data
/// </summary>
[Serializable]
public class DesignData
{
    public int m_gridWidth;
    public int m_gridHeight;
    public Room[] m_grid;
    public List<SimpleRoomData> m_simpleRoomData;
}

/// <summary>
/// Struct to hold a simplified version of a rooms data
/// </summary>
[Serializable]
public struct SimpleRoomData
{
    public int m_xPos;
    public int m_yPos;
    public string m_letterRepresentative;
    public string m_prefabName;
}

/// <summary>
/// Difficulty enum
/// </summary>
public enum Difficulty
{
    Easy,
    Medium,
    Hard,
    Count
}

/// <summary>
/// Struct to hold all of the playthrough data
/// </summary>
[Serializable]
public class PlaythroughData
{
    public int m_id;
    public List<LevelData> m_levelData;
    public int m_levelId;
}

/// <summary>
/// Struct to hold all of the level specific data
/// </summary>
[Serializable]
public class LevelData
{
    public Difficulty m_difficulty;
    public DesignData m_designData;
    public PlayerData m_playerData;
}

[Serializable]
public class BehaviouralData
{
    public float m_difficultyScore;
    public BehaviouralAverages m_scoreBehaviouralAverages;
    public BehaviouralAverages m_timeBehaviouralAverages;
    public BehaviouralAverages m_deathBehaviouralAverages;
}

[Serializable]
public class BehaviouralAverages
{
    public float m_averageValue;
    public float m_upperBoundsValue;
    public float m_lowerBoundsValue;
    public float m_standardDeviationValue;
}

/// <summary>
/// Struct to hold all of the play through stats
/// </summary>
[Serializable]
public struct OverallPlayerData
{
    public List<float> m_deaths;
    public List<float> m_scores;
    public List<float> m_times;
}

/// <summary>
/// Struct to hold multiple difficulties's overall player data
/// </summary>
[Serializable]
public struct AllOverallPlayerData
{
    public OverallPlayerData m_easy;
    public OverallPlayerData m_medium;
    public OverallPlayerData m_hard;
}