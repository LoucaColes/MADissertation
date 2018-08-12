using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An object that paths through set points
/// </summary>
public class PathingObject : MonoBehaviour
{
    // Designer variables
    [SerializeField]
    private float m_moveSpeed;

    [SerializeField]
    private Transform[] m_points;

    [SerializeField]
    private bool m_wrapAround = false;

    // Private variables
    private int m_pointIndex = 0;

    /// <summary>
    /// Initialise the object
    /// </summary>
    private void Start()
    {
        // Set the start position of the object
        transform.position = m_points[0].position;
    }

    // Update is called once per frame
    private void Update()
    {
        // Check if the distance of the object is not close to the current point
        // it is moving to
        if (Vector3.Distance(transform.position, m_points[m_pointIndex].position) >= 0.1f)
        {
            // Set the objects position using move towards
            transform.position = Vector3.MoveTowards(transform.position, m_points[m_pointIndex].position,
                m_moveSpeed * Time.deltaTime);
        }
        else // Else if its close to the current point
        {
            // Increase the point index
            m_pointIndex++;

            // If the point index is greater than the length of the points array
            if (m_pointIndex >= m_points.Length)
            {
                // If not wrapping around set the point index to 0
                if (!m_wrapAround)
                {
                    m_pointIndex = 0;
                }
                else
                {
                    // Set point index to 0
                    // Set the objects position to the first point
                    m_pointIndex = 0;
                    transform.position = m_points[0].position;
                }
            }
        }
    }
}