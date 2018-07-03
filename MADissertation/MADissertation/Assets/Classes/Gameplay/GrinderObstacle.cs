using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrinderObstacle : MonoBehaviour
{
    [SerializeField]
    private float m_rotateSpeed;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        transform.Rotate(Vector3.forward, m_rotateSpeed);
    }
}