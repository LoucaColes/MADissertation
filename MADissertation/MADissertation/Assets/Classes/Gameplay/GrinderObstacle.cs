using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A Grinder obstacle that the player has to avoid
/// </summary>
public class GrinderObstacle : MonoBehaviour
{
    // Designer variables
    [SerializeField]
    private float m_rotateSpeed;

    // Update is called once per frame
    private void Update()
    {
        // Rotate the grinder around its pivot point
        transform.Rotate(Vector3.forward, m_rotateSpeed);
    }
}