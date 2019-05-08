using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FishBehaviour : MonoBehaviour
{
    protected Rigidbody _rigidBody;

    [SerializeField]
    protected float _minSpeed = 10;
    [SerializeField]
    protected float _maxSpeed = 15;
    [SerializeField]
    protected float _turningSpeed = 10;

    // Start is called before the first frame update
    void Start()
    {
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
}
