using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAgent : MonoBehaviour
{
    [SerializeField]
    private float m_Speed;
    [SerializeField]
    private Vector3? m_Target;

    private const float TOLERANCE = 0.1f;
    
    public void SetTarget(Vector3 target)
    {
        m_Target = target;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (m_Target == null)
        {
            return;
        }

        Vector3 diff = m_Target.Value - transform.position;
        float distance = diff.magnitude;

        if (distance < TOLERANCE)
        {
            return;
        }

        Vector3 dir = diff.normalized;
        Vector3 delta = dir * (m_Speed * Time.deltaTime);
        // if speed is too large
        if (distance < delta.magnitude)
        {
            transform.Translate(diff);
        }
        else
        {
            transform.Translate(delta);
        }
    }
}
