using System.Collections;
using UnityEngine;

/// <summary>
/// One way platform class that can be used to
/// switch the platforms collider on and off
/// </summary>
public class OneWayPlatform : MonoBehaviour
{
    // Designer variables
    [SerializeField]
    private Collider2D m_solidCollider;

    /// <summary>
    /// Enable the solid collider
    /// </summary>
    public void EnableSolidCollider()
    {
        m_solidCollider.enabled = true;
    }

    /// <summary>
    /// Disable the solid collider
    /// </summary>
    public void DisableSolidCollider()
    {
        m_solidCollider.enabled = false;
    }

    /// <summary>
    /// Enable the solid collider after X seconds
    /// </summary>
    /// <returns>Wait For Seconds</returns>
    public IEnumerator EnableAfter()
    {
        yield return new WaitForSeconds(0.25f);
        EnableSolidCollider();
    }
}