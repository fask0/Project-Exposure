using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolFishLeaderBehaviour : FishBehaviour
{
    private SchoolFishSchool _school;
    private Vector3 _checkpoint;

    private bool _hasAddedItselfToSchool = false;

    [SerializeField]
    private GameObject _dummy;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _checkpoint = transform.position;
    }

    private void GetSchool()
    {
        if (transform.parent.GetComponent<SchoolFishSchool>())
        {
            _school = transform.parent.GetComponent<SchoolFishSchool>();
            _school.AddFishToSchool(this.gameObject, false, true);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _dummy.transform.LookAt(_checkpoint, Vector3.up);

        if (Vector3.Distance(transform.position, _checkpoint) < 8)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _dummy.transform.rotation, Time.fixedDeltaTime * _turningSpeed * (Vector3.Distance(transform.position, _checkpoint)));
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _dummy.transform.rotation, Time.fixedDeltaTime * _turningSpeed);
        }

        transform.position += (transform.forward * Time.fixedDeltaTime * _minSpeed);
    }

    private void Update()
    {
        if (!_hasAddedItselfToSchool)
        {
            GetSchool();
            _hasAddedItselfToSchool = true;
        }

        //Generate new checkpoint if too close
        if (InRangeOfCheckPoint())
        {
            _checkpoint = _school.GenerateNewCheckPoint(transform.position);
        }
    }

    private bool InRangeOfCheckPoint()
    {
        if (Vector3.Distance(transform.position, _checkpoint) < 2)
        {
            return true;
        }
        return false;
    }

    public Vector3 GetCheckPoint()
    {
        return _checkpoint;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_checkpoint, 2);
    }
}
