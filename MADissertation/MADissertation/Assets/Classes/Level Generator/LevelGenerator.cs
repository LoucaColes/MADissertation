using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LevelGeneration
{
    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField]
        private CameraMovement m_cameraMovement;

        [SerializeField]
        private int m_gridWidth = 4;

        [SerializeField]
        private int m_gridHeight = 4;

        [SerializeField]
        private TemplateHolder m_templateHolder;

        [SerializeField]
        private GameObject m_emptyRoomPrefab;

        [SerializeField]
        private Vector2 m_positioningOffset = new Vector2(17.2f, 9.2f);

        [SerializeField]
        private GameObject m_playerPref;

        [SerializeField]
        private bool m_debugMode = false;

        private Grid m_grid;

        // Use this for initialization
        private void Start()
        {
            m_grid = gameObject.AddComponent<LevelGeneration.Grid>();
            int difficulty = (int)DataTracker.Instance.GetDifficulty();
            TemplateGroup templateGroup;
            if (difficulty == 0)
            {
                templateGroup = m_templateHolder.GetEasyGroup();
            }
            else if (difficulty == 1)
            {
                templateGroup = m_templateHolder.GetMediumGroup();
            }
            else
            {
                templateGroup = m_templateHolder.GetHardGroup();
            }
            int randWidthHeight = Random.Range(templateGroup.m_gridMinMax.x,
                templateGroup.m_gridMinMax.y);
            m_gridWidth = randWidthHeight;
            m_gridHeight = randWidthHeight;
            m_grid.CreateGrid(randWidthHeight, randWidthHeight, m_emptyRoomPrefab, m_positioningOffset, m_templateHolder,
                m_cameraMovement, m_playerPref);
            m_grid.GenerateLevel();
        }

        // Update is called once per frame
        private void Update()
        {
            if (m_debugMode)
            {
                if (Input.GetKeyDown(KeyCode.G))
                {
                    m_grid.GenerateLevel();
                }
                if (Input.GetKeyDown(KeyCode.C))
                {
                    m_grid.Clear();
                }
            }
        }
    }
}