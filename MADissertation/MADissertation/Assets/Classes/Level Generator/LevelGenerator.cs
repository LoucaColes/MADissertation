using UnityEngine;

namespace LevelGeneration
{
    /// <summary>
    /// Level generator class that manages the generation process
    /// </summary>
    public class LevelGenerator : MonoBehaviour
    {
        // Designer variables
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

        // Private variables
        private Grid m_grid;

        // Use this for initialization
        private void Start()
        {
            // Add a new grid class to the level generation object
            m_grid = gameObject.AddComponent<LevelGeneration.Grid>();

            // Get the template group based on the current difficulty
            TemplateGroup templateGroup = m_templateHolder.GeTemplateGroup(DataTracker.Instance.GetDifficulty());

            // Generate a random width and height based on the template groups min max values
            int randWidthHeight = Random.Range(templateGroup.m_gridMinMax.x,
                templateGroup.m_gridMinMax.y);

            // Set the grids width and height
            m_gridWidth = randWidthHeight;
            m_gridHeight = randWidthHeight;

            // Create a new grid
            m_grid.CreateGrid(randWidthHeight, randWidthHeight, m_emptyRoomPrefab, m_positioningOffset, m_templateHolder, m_playerPref);

            // Generate a new level
            m_grid.GenerateLevel();
        }

        // Update is called once per frame
        private void Update()
        {
            // If in debug mode
            if (m_debugMode)
            {
                // If the G key is pressed generate a level
                if (Input.GetKeyDown(KeyCode.G))
                {
                    m_grid.GenerateLevel();
                }
                // If the C key is pressed clear the grid
                if (Input.GetKeyDown(KeyCode.C))
                {
                    m_grid.Clear();
                }
            }
        }
    }
}