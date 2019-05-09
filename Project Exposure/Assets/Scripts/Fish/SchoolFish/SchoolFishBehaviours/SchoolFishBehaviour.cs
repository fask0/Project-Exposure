using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolFishBehaviour : FishBehaviour
{
    private FishZone _school;
    private SchoolFishLeaderBehaviour _schoolFishLeaderBehaviour;

    private bool _hasAddedItselfToSchool = false;
    private bool _fishTooClose = false;
    private GameObject _fishThatsTooClose;
    private FishBehaviour _fishThatsTooCloseBehaviour;

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
        if (transform.parent.GetComponent<FishZone>())
        {
            _school = transform.parent.GetComponent<FishZone>();
            _school.AddFishToSchool(this.gameObject);
            transform.position = _schoolFishLeaderBehaviour.transform.position + new Vector3(SingleTons.GameController.GetRandomRange(0, 2), SingleTons.GameController.GetRandomRange(0, 2), SingleTons.GameController.GetRandomRange(0, 2));
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

        SpeedUpAndDown();
        if (_fishTooClose)
        {
            transform.position += (transform.forward * Time.fixedDeltaTime * _currentSpeed);
        }
        else
        {
            transform.position += (transform.forward * Time.fixedDeltaTime * _currentSpeed);
        }
    }

    private void SpeedUpAndDown()
    {
        if (_fishTooClose)
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, Mathf.Min(_minSpeed * _fishThatsTooCloseBehaviour.GetThreatLevel(), _maxSpeed), Time.fixedDeltaTime * _speedUpRate);
        }
        else
        {
            _currentSpeed = Mathf.Lerp(_currentSpeed, _minSpeed, Time.fixedDeltaTime * _speedUpRate);
        }
    }

    private void RotateTowardsCheckPoint()
    {
        if (_hasAddedItselfToSchool)
        {
            _dummy.transform.LookAt(_schoolFishLeaderBehaviour.GetCheckPoint(), Vector3.up);

            transform.rotation = Quaternion.Slerp(transform.rotation, _dummy.transform.rotation, Time.fixedDeltaTime * _turningSpeed);
        }
    }

    private void RotateAwayFromFish()
    {
        _dummy.transform.LookAt(Reflect(_schoolFishLeaderBehaviour.GetCheckPoint(), _fishThatsTooClose.transform.position), Vector3.up);

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
            List<GameObject> fishToAvoid = _school.GetFishToAvoid();
            List<FishBehaviour> fishToAvoidBehaviours = _school.GetFishToAvoidBehaviours();
            for (int i = 0; i < fishToAvoid.Count; i++)
            {
                if (fishToAvoid[i] != gameObject)
                {
                    if (Vector3.Distance(transform.position, fishToAvoid[i].transform.position) < fishToAvoidBehaviours[i].GetThreatRange())
                    {
                        _fishThatsTooClose = fishToAvoid[i];
                        _fishThatsTooCloseBehaviour = fishToAvoidBehaviours[i];
                        _fishTooClose = true;
                        return;
                    }
                }
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, _fishThatsTooClose.transform.position) > _fishThatsTooCloseBehaviour.GetThreatFleeRange())
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

    public void SetSchoolFishLeader(SchoolFishLeaderBehaviour schoolFishLeaderBehaviour)
    {
        _schoolFishLeaderBehaviour = schoolFishLeaderBehaviour;
    }
}
