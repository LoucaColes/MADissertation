using LevelGeneration;
using UnityEngine;

/// <summary>
/// A scriptable object that can be used to store the base averages that
/// are used when a new profile is created on the database
/// </summary>
[CreateAssetMenu(fileName = "BaseAverages", menuName = "Averages/BaseAverages", order = 1)]
public class BaseAverages : ScriptableObject
{
    [SerializeField]
    private Averages m_easyAverages;

    [SerializeField]
    private Averages m_mediumAverages;

    [SerializeField]
    private Averages m_hardAverages;

    /// <summary>
    /// Get the easy averages
    /// </summary>
    /// <returns>Easy Averages</returns>
    public Averages GetEasyAverages()
    {
        return m_easyAverages;
    }

    /// <summary>
    /// Get the medium averages
    /// </summary>
    /// <returns>Medium Averages</returns>
    public Averages GetMediumAverages()
    {
        return m_mediumAverages;
    }

    /// <summary>
    /// Get the hard averages
    /// </summary>
    /// <returns>Hard Averages</returns>
    public Averages GetHardAverages()
    {
        return m_hardAverages;
    }
}