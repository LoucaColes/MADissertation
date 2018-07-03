using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LevelGeneration;
using UnityEngine;
using Utilities;

/// <summary>
/// Class to handle the data tracking over the playthrough of the demo
/// </summary>
public class DataTracker : Singleton<DataTracker>
{
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

    // List of all tracked data
    private List<PlayerData> m_playerDataList = new List<PlayerData>();
    private List<LevelData> m_levelDataList = new List<LevelData>();
    private List<Difficulty> m_difficultyDataList = new List<Difficulty>();

    private Difficulty m_currentDifficulty;

    protected override void Awake()
    {
        base.Awake();
        RandomiseDifficulty();
    }

    /// <summary>
    /// Update function that can be used for debugging
    /// </summary>
    private void Update()
    {
        // If in debug mode
        if (m_debugMode)
        {
            // If debug export button pressed call ExportData()
            if (Input.GetKeyDown(m_debugExportButton))
            {
                Debug.Log("Debug Exporting");
                ExportData();
            }
        }
    }

    /// <summary>
    /// Adds new data to the tracked data list
    /// </summary>
    /// <param name="_data"></param>
    public void AddNewPlayerData(ref PlayerData _data)
    {
        // Add new data to the tracked data list
        m_playerDataList.Add(_data);
    }

    public void AddNewLevelData(ref LevelData _levelData)
    {
        m_levelDataList.Add(_levelData);
    }

    /// <summary>
    /// Called when the object is destroyed and used to export data
    /// </summary>
    protected override void OnDestroy()
    {
        // Call the base singletons OnDestroy()
        base.OnDestroy();

        // Export the tracked data
        ExportData();
    }

    /// <summary>
    /// Exports all tracked data
    /// </summary>
    private void ExportData()
    {
        // Check if there is any data to export
        if (m_playerDataList.Count <= 0)
        {
            // If not don't export anything
            Debug.LogError("No Data To Export");
        }
        // Check if there is any data to export
        if (m_levelDataList.Count <= 0)
        {
            // If not don't export anything
            Debug.LogError("No Data To Export");
        }
        if (m_playerDataList.Count > 0 && m_levelDataList.Count > 0)
        {
            ExportJsonData();
        }
    }

    private void ExportJsonData()
    {
        // Convert lists to an array and in JSON format
        string[] convertedDataArray = ConvertPlayerData();

        string[] convertedLevelData = ConvertLevelData();

        string[] convertedDifficultyData = ConvertDifficultyData();

        // Create the full file path
        string filePath = Application.dataPath + "/" + m_fileName + ".txt";

        string[] mergedData = MergeJsonData(convertedDataArray, convertedLevelData, convertedDifficultyData);

        // Write all data in the string array to the text file
        File.WriteAllLines(filePath, mergedData);
    }

    public void RandomiseDifficulty()
    {
        int rand = UnityEngine.Random.Range(0, (int)Difficulty.Count);
        m_currentDifficulty = (Difficulty)rand;
        m_difficultyDataList.Add(m_currentDifficulty);
    }

    public Difficulty GetDifficulty()
    {
        return m_currentDifficulty;
    }

    private string[] ConvertPlayerData()
    {
        // Create a new list to store all the converted data
        List<string> convertedData = new List<string>();

        // Convert data into a JSON string and add to the list
        for (int index = 0; index < m_playerDataList.Count; index++)
        {
            string jsonData = JsonUtility.ToJson(m_playerDataList[index]);
            convertedData.Add(jsonData);
        }

        return convertedData.ToArray();
    }

    private string[] ConvertLevelData()
    {
        List<string> allLevelDataString = new List<string>();
        for (int levelIndex = 0; levelIndex < m_levelDataList.Count; levelIndex++)
        {
            string levelDataString = "";
            levelDataString += m_levelDataList[levelIndex].m_gridWidth.ToString() + "\n" + m_levelDataList[levelIndex].m_gridHeight.ToString() + "\n";
            List<SimpleRoomData> tempRoomData = m_levelDataList[levelIndex].m_simpleRoomData;
            foreach (SimpleRoomData roomData in tempRoomData)
            {
                if (roomData.m_xPos > (0))
                {
                    levelDataString += roomData.m_letterRepresentative;
                }
                else
                {
                    levelDataString += (roomData.m_letterRepresentative + "\n");
                }
            }
            allLevelDataString.Add(levelDataString);
        }

        return allLevelDataString.ToArray();
    }

    private string[] ConvertDifficultyData()
    {
        List<string> convertedDifficultyData = new List<string>();

        // Convert data into a JSON string and add to the list
        for (int index = 0; index < m_difficultyDataList.Count; index++)
        {
            string jsonData = m_difficultyDataList[index].ToString();
            convertedDifficultyData.Add(jsonData);
        }

        return convertedDifficultyData.ToArray();
    }

    private string[] MergeJsonData(string[] _playerData, string[] _levelData, string[] _difficultyData)
    {
        List<string> mergedData = new List<string>();

        if (_playerData.Length == _levelData.Length)
        {
            for (int index = 0; index < _playerData.Length; index++)
            {
                mergedData.Add(_levelData[index]);
                mergedData.Add("\n");
                mergedData.Add(_difficultyData[index]);
                mergedData.Add("\n");
                mergedData.Add(_playerData[index]);
                mergedData.Add("\n");
            }
        }
        else
        {
            Debug.LogWarning("Array Lengths don't match and therefore data might be lost");
            for (int index = 0; index < _levelData.Length; index++)
            {
                mergedData.Add(_levelData[index]);
                mergedData.Add("\n");
                mergedData.Add(_difficultyData[index]);
                mergedData.Add("\n");

                if (index < _playerData.Length)
                {
                    mergedData.Add(_playerData[index]);
                    mergedData.Add("\n");
                }
                else
                {
                    mergedData.Add("Level wasn't finished so there is no player data.");
                    mergedData.Add("\n");
                }
            }
        }

        return mergedData.ToArray();
    }
}

/// <summary>
/// Struct to hold all of the tracked data
/// </summary>
[System.Serializable]
public struct PlayerData
{
    public int m_currentPlayerLives;
    public int m_currentPlayerDeaths;
    public float m_timeTakenToCompleteLevel;
    public int m_enemiesKilled;
    public int m_score;
}

[Serializable]
public struct LevelData
{
    public int m_gridWidth;
    public int m_gridHeight;
    public Room[] m_grid;
    public List<SimpleRoomData> m_simpleRoomData;
}

[Serializable]
public struct SimpleRoomData
{
    public int m_xPos;
    public int m_yPos;
    public string m_letterRepresentative;
}

public enum Difficulty
{
    Easy,
    Medium,
    Hard,
    Count
}