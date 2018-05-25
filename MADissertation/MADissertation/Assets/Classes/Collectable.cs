using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField]
    private int m_scoreValue;

    public int Collect()
    {
        return m_scoreValue;
    }
}