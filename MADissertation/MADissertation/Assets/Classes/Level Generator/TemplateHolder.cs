using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelGeneration
{
    [CreateAssetMenu(fileName = "TemplateHolder", menuName = "Templates/Holder", order = 1)]
    public class TemplateHolder : ScriptableObject
    {
        [SerializeField]
        private TemplateGroup m_easyGroup;

        [SerializeField]
        private TemplateGroup m_mediumGroup;

        [SerializeField]
        private TemplateGroup m_hardGroup;

        public TemplateGroup GetEasyGroup()
        {
            return m_easyGroup;
        }

        public TemplateGroup GetMediumGroup()
        {
            return m_mediumGroup;
        }

        public TemplateGroup GetHardGroup()
        {
            return m_hardGroup;
        }
    }

    [System.Serializable]
    public class TemplateGroup
    {
        public string m_groupName;
        public Difficulty m_difficulty;
        public GameObject[] m_roomPrefabs;
        public Vector2Int m_gridMinMax;
    }
}