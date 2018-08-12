using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelGeneration
{
    /// <summary>
    /// A scriptable object that holds all of the different difficulty groups data
    /// such as the room prefabs, grid sizes and averages
    /// </summary>
    [CreateAssetMenu(fileName = "TemplateHolder", menuName = "Templates/Holder", order = 1)]
    public class TemplateHolder : ScriptableObject
    {
        // Designer variables
        [SerializeField]
        private List<TemplateGroup> m_templateGroups;

        /// <summary>
        /// Access all of the template groups
        /// </summary>
        /// <returns>A list of template groups</returns>
        public List<TemplateGroup> GetTemplateGroups()
        {
            return m_templateGroups;
        }

        /// <summary>
        /// Access a specific template group
        /// </summary>
        /// <param name="_currentDifficulty">The current difficulty</param>
        /// <returns>A specific template group</returns>
        public TemplateGroup GeTemplateGroup(Difficulty _currentDifficulty)
        {
            // Set a template group variable to null incase of invalid difficulty
            TemplateGroup templateGroup = null;

            // Loop through all of the template groups
            for (int index = 0; index < m_templateGroups.Count; index++)
            {
                // If the template group's difficulty matches the passed in difficulty
                if (m_templateGroups[index].m_difficulty == _currentDifficulty)
                {
                    // Return the template group
                    templateGroup = m_templateGroups[index];
                    return templateGroup;
                }
            }

            // Return template group
            return templateGroup;
        }
    }

    /// <summary>
    /// A class that stores data related to the groups difficulty
    /// </summary>
    [System.Serializable]
    public class TemplateGroup
    {
        public string m_groupName;
        public Difficulty m_difficulty;
        public GameObject[] m_roomPrefabs;
        public Vector2Int m_gridMinMax;
        public int m_collectableCount;
        public DataAverages m_deathsAverages;
        public DataAverages m_scoreAverages;
        public DataAverages m_timeAverages;
    }

    /// <summary>
    /// A struct that stores an average and standard deviation value
    /// </summary>
    [System.Serializable]
    public struct DataAverages
    {
        public float m_average;
        public float m_standardDeviation;
    }

    /// <summary>
    /// A struct that stores data averages for specific stats
    /// </summary>
    [System.Serializable]
    public struct Averages
    {
        public DataAverages m_deathsAverages;
        public DataAverages m_scoreAverages;
        public DataAverages m_timeAverages;
    }
}