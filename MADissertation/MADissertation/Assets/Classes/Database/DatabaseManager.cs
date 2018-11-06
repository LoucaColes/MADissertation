using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using LevelGeneration;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using Utilities;

/// <summary>
/// Singleton class to handle the linking between the game and the firebase database
/// </summary>
public class DatabaseManager : Singleton<DatabaseManager>
{
    // Designer Variables
    [SerializeField]
    private IdType m_idType;

    [SerializeField]
    private string m_groupName;

    [SerializeField]
    private BaseAverages m_baseAverages;

    [SerializeField]
    private bool m_startDifficultyOverride = true;

    [SerializeField]
    private Difficulty m_startDifficulty = Difficulty.Easy;

    [SerializeField]
    private bool m_randomiseDifficulty = false;

    [SerializeField] private GameObject m_canvas;

    // Private variables
    private DatabaseReference m_database;
    private bool m_databaseReady = false;
    private int m_playthroughId;
    private Averages m_initEasyAverages;
    private Averages m_initMedAverages;
    private Averages m_initHardAverages;
    private string m_uniqueId;
    private Task initTask;

    /// <summary>
    /// Database initialisation and singleton set up
    /// </summary>
    protected override void Awake()
    {
        // Singleton set up
        base.Awake();

        // Get the device's unique ID
        m_uniqueId = SystemInfo.deviceUniqueIdentifier;

        // Check if the database is available and connect to the database
        initTask = Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Set a flag here indiciating that Firebase is ready to use by your
                // application.
                FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://ma-dissertation.firebaseio.com/");
                // Get the root reference location of the database.
                m_database = FirebaseDatabase.DefaultInstance.RootReference;
                // If connected set database to ready
                m_databaseReady = true;
                Debug.Log("Database is now ready attempt 1");

                // Access database and check if there is a profile that matches the used ID
                m_database.Child(AccessId())
                    .GetValueAsync()
                    .ContinueWith(countTask =>
                    {
                        // Check for any faults
                        if (countTask.IsFaulted)
                        {
                            // Handle the error...
                            Debug.LogError("Failed to get ID");
                        }
                        // If no faults
                        else if (countTask.IsCompleted)
                        {
                            // Get the data snapshot from the server
                            DataSnapshot snapshot = countTask.Result;

                            // If there is pre exisiting data
                            if (snapshot.Exists)
                            {
                                Debug.Log("ID Exists");

                                // Get the playthrough count
                                StartCoroutine(SetUpCount());

                                // Get the averages
                                StartCoroutine(GetInitEasyAverages());
                                StartCoroutine(GetInitMedAverages());
                                StartCoroutine(GetInitHardAverages());

                                // Set the menu canvas to active
                                m_canvas.SetActive(true);
                            }
                            else
                            {
                                // If no pre exisitng data, set baseline averages
                                // and a playthrough count of 0
                                Debug.LogError("Failed to get ID, setting up baseline averages");
                                SetBaseLineAveragesWOWait();

                                // Set the menu canvas to active
                                m_canvas.SetActive(true);
                            }
                        }
                    });
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    /// <summary>
    /// Get playthough count from the database
    /// </summary>
    /// <returns>Null</returns>
    private IEnumerator SetUpCount()
    {
        // If database is not ready then return null until it is
        while (!m_databaseReady)
        {
            yield return null;
        }
        // When database is ready then retreive the playthrough count value
        m_database.Child(AccessId()).Child("PlaythroughCount").GetValueAsync().ContinueWith(countTask =>
            {
                // Check for any faults
                if (countTask.IsFaulted)
                {
                    // Handle the error...
                    Debug.LogError("Failed to get playthrough count");
                }
                // If no faults
                else if (countTask.IsCompleted)
                {
                    DataSnapshot snapshot = countTask.Result;
                    // Parse json value for playthrough count and set value
                    m_playthroughId = int.Parse(snapshot.GetRawJsonValue());
                }
            });
    }

    /// <summary>
    /// Accesses the database to get the easy averages
    /// </summary>
    /// <returns>Null</returns>
    private IEnumerator GetInitEasyAverages()
    {
        // If database is not ready then return null until it is
        while (!m_databaseReady)
        {
            yield return null;
        }
        // When database is ready then retreive the easy averages
        m_database.Child(AccessId()).Child("Averages").Child("Easy Averages").GetValueAsync().ContinueWith(countTask =>
        {
            // Check for any faults
            if (countTask.IsFaulted)
            {
                // Handle the error...
                Debug.LogError("Failed to get easy averages");
            }
            // If no faults
            else if (countTask.IsCompleted)
            {
                DataSnapshot snapshot = countTask.Result;
                // Parse json value for averages and set value
                string rawJson = snapshot.GetRawJsonValue();
                m_initEasyAverages = JsonUtility.FromJson<Averages>(rawJson);
            }
        });
    }

    /// <summary>
    /// Accesses the database and gets the medium averages
    /// </summary>
    /// <returns>Null</returns>
    private IEnumerator GetInitMedAverages()
    {
        // If database is not ready then return null until it is
        while (!m_databaseReady)
        {
            yield return null;
        }
        // When database is ready then retreive the medium averages
        m_database.Child(AccessId()).Child("Averages").Child("Medium Averages").GetValueAsync().ContinueWith(countTask =>
        {
            // Check for any faults
            if (countTask.IsFaulted)
            {
                // Handle the error...
                Debug.LogError("Failed to get medium averages");
            }
            // If no faults
            else if (countTask.IsCompleted)
            {
                DataSnapshot snapshot = countTask.Result;
                // Parse json value for averages and set value
                string rawJson = snapshot.GetRawJsonValue();
                m_initMedAverages = JsonUtility.FromJson<Averages>(rawJson);
            }
        });
    }

    /// <summary>
    /// Accesses the database and retreives the hard averages
    /// </summary>
    /// <returns></returns>
    private IEnumerator GetInitHardAverages()
    {
        // If database is not ready then return null until it is
        while (!m_databaseReady)
        {
            yield return null;
        }
        // When database is ready then retreive the hard averages
        m_database.Child(AccessId()).Child("Averages").Child("Hard Averages").GetValueAsync().ContinueWith(countTask =>
        {
            // Check for any faults
            if (countTask.IsFaulted)
            {
                // Handle the error...
                Debug.LogError("Failed to get hard averages");
            }
            // If no faults
            else if (countTask.IsCompleted)
            {
                DataSnapshot snapshot = countTask.Result;
                // Parse json value for averages and set value
                string rawJson = snapshot.GetRawJsonValue();
                m_initHardAverages = JsonUtility.FromJson<Averages>(rawJson);
            }
        });
    }

    /// <summary>
    /// Set the baseline averages for the new profile
    /// </summary>
    /// <returns>Null</returns>
    private IEnumerator SetBaseLineAverages()
    {
        // While database not ready return null
        while (!m_databaseReady)
        {
            yield return null;
        }

        // If database is ready
        if (m_databaseReady)
        {
            // Convert base averages to JSON
            string rawEasyJson = JsonUtility.ToJson(m_baseAverages.GetEasyAverages());
            string rawMediumJson = JsonUtility.ToJson(m_baseAverages.GetMediumAverages());
            string rawHardJson = JsonUtility.ToJson(m_baseAverages.GetHardAverages());

            // Set the new profiles easy averages
            m_database.Child(AccessId()).
                Child("Averages").Child("Easy Averages").SetRawJsonValueAsync(rawEasyJson);

            // Set the new profiles medium averages
            m_database.Child(AccessId()).
                Child("Averages").Child("Medium Averages").SetRawJsonValueAsync(rawMediumJson);

            // Set the new profiles hard averages
            m_database.Child(AccessId()).
                Child("Averages").Child("Hard Averages").SetRawJsonValueAsync(rawHardJson);

            // Set the new profiles playthrough count
            m_database.Child(AccessId()).
                Child("PlaythroughCount")
                .SetValueAsync(0);
            m_playthroughId = 0;

            // Set the initial averages to the base averages
            m_initEasyAverages = m_baseAverages.GetEasyAverages();
            m_initMedAverages = m_baseAverages.GetMediumAverages();
            m_initHardAverages = m_baseAverages.GetHardAverages();
        }
    }

    /// <summary>
    /// Set the base line averahes without a wait
    /// </summary>
    private void SetBaseLineAveragesWOWait()
    {
        // If database is ready
        if (m_databaseReady)
        {
            // Convert base averages to JSON
            string rawEasyJson = JsonUtility.ToJson(m_baseAverages.GetEasyAverages());
            string rawMediumJson = JsonUtility.ToJson(m_baseAverages.GetMediumAverages());
            string rawHardJson = JsonUtility.ToJson(m_baseAverages.GetHardAverages());

            // Set the new profiles easy averages
            m_database.Child(AccessId()).
                Child("Averages").Child("Easy Averages").SetRawJsonValueAsync(rawEasyJson);

            // Set the new profiles medium averages
            m_database.Child(AccessId()).
                Child("Averages").Child("Medium Averages").SetRawJsonValueAsync(rawMediumJson);

            // Set the new profiles hard averages
            m_database.Child(AccessId()).
                Child("Averages").Child("Hard Averages").SetRawJsonValueAsync(rawHardJson);

            // Set the new profiles playthrough count
            m_database.Child(AccessId()).
                Child("PlaythroughCount")
                .SetValueAsync(0);
            m_playthroughId = 0;

            // Set the initial averages to the base averages
            m_initEasyAverages = m_baseAverages.GetEasyAverages();
            m_initMedAverages = m_baseAverages.GetMediumAverages();
            m_initHardAverages = m_baseAverages.GetHardAverages();
        }
    }

    /// <summary>
    /// Allow other classes to access the database reference
    /// </summary>
    /// <returns>Returns the reference to the database</returns>
    public DatabaseReference GetDatabaseReference()
    {
        return m_database;
    }

    /// <summary>
    /// Allows other classes to check if the database is ready
    /// </summary>
    /// <returns>Returns if the database is ready or not</returns>
    public bool IsDatabaseReady()
    {
        return m_databaseReady;
    }

    /// <summary>
    /// Allows other classes to access the playthrough id
    /// </summary>
    /// <returns>Returns the current playthrough count</returns>
    public int GetPlaythroughId()
    {
        return m_playthroughId;
    }

    /// <summary>
    /// Increases the playthrough count
    /// </summary>
    public void IncreasePlaythroughId()
    {
        m_playthroughId++;
    }

    /// <summary>
    /// Access the easy averages
    /// </summary>
    /// <returns>Easy averages</returns>
    public Averages AccessInitEasyAverages()
    {
        return m_initEasyAverages;
    }

    /// <summary>
    /// Access the medium averages
    /// </summary>
    /// <returns>Medium averages</returns>
    public Averages AccessInitMediumAverages()
    {
        return m_initMedAverages;
    }

    /// <summary>
    /// Access the hard averages
    /// </summary>
    /// <returns>Hard averages</returns>
    public Averages AccessInitHardAverages()
    {
        return m_initHardAverages;
    }

    /// <summary>
    /// Access the group ID
    /// </summary>
    /// <returns>Group ID</returns>
    public string AccessGroupName()
    {
        return m_groupName;
    }

    /// <summary>
    /// Access the unique device ID
    /// </summary>
    /// <returns>Unique device ID</returns>
    public string AccessUniqueId()
    {
        return m_uniqueId;
    }

    /// <summary>
    /// Access the player ID
    /// </summary>
    /// <returns>Returns the group ID or the unique ID</returns>
    public string AccessId()
    {
        if (m_idType == IdType.Group)
        {
            return m_groupName;
        }
        else
        {
            return m_uniqueId;
        }
    }

    /// <summary>
    /// Check if there is an override on the start level difficulty
    /// </summary>
    /// <returns>Returns the start difficulty override bool</returns>
    public bool IsStartDifficultyOverride()
    {
        return m_startDifficultyOverride;
    }

    /// <summary>
    /// Get the start difficulty that overrides the original one
    /// </summary>
    /// <returns>Returns the start difficulty override</returns>
    public Difficulty StartDifficulty()
    {
        return m_startDifficulty;
    }

    /// <summary>
    /// Check if the difficulty should be randomised
    /// </summary>
    /// <returns>Returns if the difficulty is randomised</returns>
    public bool IsDifficultyRandomised()
    {
        return m_randomiseDifficulty;
    }
}

/// <summary>
/// Type of ID the player is using
/// </summary>
public enum IdType
{
    Group,
    Solo
}