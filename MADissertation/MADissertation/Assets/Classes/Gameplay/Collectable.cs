using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that a player can pick up to get a higher score
/// </summary>
public class Collectable : MonoBehaviour
{
    // Designer Variables
    [SerializeField]
    private int m_scoreValue;

    /// <summary>
    /// Returns a score value to be added to the overall score
    /// </summary>
    /// <returns>The amount of points given when picking up this object</returns>
    public int Collect()
    {
        return m_scoreValue;
    }
}