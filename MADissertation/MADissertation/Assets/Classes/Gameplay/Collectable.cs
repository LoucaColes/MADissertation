using UnityEngine;

/// <summary>
/// An object that a player can pick up to get a higher score
/// </summary>
public class Collectable : MonoBehaviour
{
    // Designer Variables
    [SerializeField]
    private int m_scoreValue;

    [SerializeField]
    private int m_spawnPercentage;

    /// <summary>
    /// Returns a score value to be added to the overall score
    /// </summary>
    /// <returns>The amount of points given when picking up this object</returns>
    public int Collect()
    {
        return m_scoreValue;
    }

    /// <summary>
    /// Returns the spawn rate for the specific collectable
    /// </summary>
    /// <returns>Spawn Rate</returns>
    public int GetSpawnRate()
    {
        return m_spawnPercentage;
    }
}