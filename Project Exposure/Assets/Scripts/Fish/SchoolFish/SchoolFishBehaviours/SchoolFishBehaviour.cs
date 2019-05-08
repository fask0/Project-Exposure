using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolFishBehaviour : FishBehaviour
{
    private SchoolFishSchool _school;

    private bool _hasAddedItselfToSchool = false;
    private bool _fishTooClose = false;
    private GameObject _fishThatsTooClose;

    [SerializeField]
    private float _avoidFishRange = 3;
    [SerializeField]
    private float _keepAvoidingFishRange = 5;
    [SerializeField]
    private GameObject _dummy;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    private void GetSchool()
    {
        if (transform.parent.GetComponent<SchoolFishSchool>())
        {
            _school = transform.parent.GetComponent<SchoolFishSchool>();
            _school.AddFishToSchool(this.gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!_fishTooClose)
        {
            RotateTowardsCheckPoint();
        }
        else
        {
            RotateAwayFromFish();
        }

        transform.position += (transform.forward * Time.fixedDeltaTime * _minSpeed);
    }

    private void RotateTowardsCheckPoint()
    {
        if (_hasAddedItselfToSchool)
        {
            _dummy.transform.LookAt(_school._leaderBehaviour.GetCheckPoint(), Vector3.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, _dummy.transform.rotation, Time.fixedDeltaTime * _turningSpeed);
        }
    }

    private void RotateAwayFromFish()
    {
        _dummy.transform.LookAt(Reflect(_school._leaderBehaviour.GetCheckPoint(), _fishThatsTooClose.transform.position), Vector3.up);

        transform.rotation = Quaternion.Slerp(transform.rotation, _dummy.transform.rotation, Time.fixedDeltaTime * _turningSpeed);
    }

    private void Update()
    {
        if (!_hasAddedItselfToSchool)
        {
            GetSchool();
            _hasAddedItselfToSchool = true;
        }

        if (!_fishTooClose)
        {
            foreach (GameObject fish in _school.GetFish())
            {
                if (fish != gameObject)
                {
                    if (Vector3.Distance(transform.position, fish.transform.position) < _avoidFishRange)
                    {
                        _fishThatsTooClose = fish;
                        _fishTooClose = true;
                        return;
                    }
                }
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, _fishThatsTooClose.transform.position) > _keepAvoidingFishRange)
            {
                _fishTooClose = false;
                return;
            }
            else
            {
                return;
            }
        }
        _fishTooClose = false;
    }

    private Vector3 Reflect(Vector3 _checkPoint, Vector3 _otherFishPos)
    {
        Vector3 diff = transform.position - _otherFishPos;
        Vector3 subtractingValue = (_checkPoint - transform.position).normalized * diff.magnitude;

        return transform.position + (diff * 2 - new Vector3(0, 0, subtractingValue.z));
    }
}
