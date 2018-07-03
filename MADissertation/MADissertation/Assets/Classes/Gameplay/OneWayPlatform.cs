using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    [SerializeField]
    private Collider2D m_solidCollider;

    public void EnableSolidCollider()
    {
        m_solidCollider.enabled = true;
    }

    public void DisableSolidCollider()
    {
        m_solidCollider.enabled = false;
    }

    public IEnumerator EnableAfter()
    {
        yield return new WaitForSeconds(0.25f);
        EnableSolidCollider();
    }
}