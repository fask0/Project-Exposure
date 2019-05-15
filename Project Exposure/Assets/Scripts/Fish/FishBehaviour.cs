using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FishBehaviour : FishBehaviourParent
{
    protected Rigidbody _rigidBody;

    [SerializeField]
    protected float _minSpeed = 10;
    [SerializeField]
    protected float _maxSpeed = 15;
    [SerializeField]
    protected float _turningSpeed = 10;
    [SerializeField]
    [Range(0, 100)]
    protected float _speedUpRate = 10;
    [SerializeField]
    protected GameObject _dummy;

    protected float _currentSpeed;

    // Start is called before the first frame update
    void Start()
    {
        _currentSpeed = _minSpeed;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public float GetMinSpeed()
    {
        return _minSpeed;
    }

    public float GetMaxSpeed()
    {
        return _maxSpeed;
    }

    public float GetTurningSpeed()
    {
        return _turningSpeed;
    }

    public GameObject GetDummy()
    {
        return _dummy;
    }

    //public float GetThreatRange()
    //{
    //    return _threatRange;
    //}

    //public float GetThreatFleeRange()
    //{
    //    return _threatFleeRange;
    //}

    //public float GetThreatLevel()
    //{
    //    return _threatLevel;
    //}

    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireSphere(transform.position, _threatRange);

    //    Gizmos.color = Color.cyan;
    //    Gizmos.DrawWireSphere(transform.position, _threatFleeRange);
    //}
}
