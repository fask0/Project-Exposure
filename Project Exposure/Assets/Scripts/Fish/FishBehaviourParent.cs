﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBehaviourParent : MonoBehaviour
{
    [SerializeField]
    [Range(0, 50)]
    protected float _threatRange = 5;
    [SerializeField]
    [Range(0, 50)]
    protected float _threatFleeRange = 5;
    [SerializeField]
    [Range(0, 10)]
    protected float _threatLevel;
    [SerializeField]
    private bool _drawRange = false;

    public float GetThreatRange()
    {
        return _threatRange;
    }

    public float GetThreatFleeRange()
    {
        return _threatFleeRange;
    }

    public float GetThreatLevel()
    {
        return _threatLevel;
    }

    private void OnDrawGizmosSelected()
    {
        if (_drawRange)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _threatRange);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _threatFleeRange);
        }
    }
}
