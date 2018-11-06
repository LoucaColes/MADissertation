using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A Canvas class that controls aspects of the game UI
/// </summary>
public class GameCanvas : MonoBehaviour
{
    // Designer variables
    [SerializeField]
    private Text m_respawnText;

    [SerializeField]
    private Text m_levelText;

    [SerializeField]
    private Text m_deathsText;

    [SerializeField]
    private Text m_timeText;

    [SerializeField]
    private Text m_difficultyText;

    [SerializeField]
    private Text m_scoreText;

    /// <summary>
    /// Update the level text object
    /// </summary>
    /// <param name="_levelId">Level ID value</param>
    public void UpdateLevelText(int _levelId)
    {
        m_levelText.text = "Level: " + _levelId;
    }

    /// <summary>
    /// Update the deaths text object
    /// </summary>
    /// <param name="_deathCount">Death count value</param>
    public void UpdateDeathsText(int _deathCount)
    {
        m_deathsText.text = "Deaths: " + _deathCount;
    }

    /// <summary>
    /// Update the time text object
    /// </summary>
    /// <param name="_time">Time value</param>
    public void UpdateTimeText(float _time)
    {
        m_timeText.text = "Time: " + _time.ToString("F2");
    }

    /// <summary>
    /// Update the difficulty text object
    /// </summary>
    /// <param name="_difficulty">Difficulty Value</param>
    public void UpdateDifficultyText(Difficulty _difficulty)
    {
        m_difficultyText.text = "Difficulty: " + _difficulty.ToString();
    }

    /// <summary>
    /// Update the score text object
    /// </summary>
    /// <param name="_score">Score</param>
    public void UpdateScoreText(int _score)
    {
        m_scoreText.text = "Score: " + _score;
    }
}