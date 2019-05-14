﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolFishLeaderBehaviour : FishBehaviour
{
    private FishZone _school;
    private Vector3 _checkpoint;

    private bool _hasAddedItselfToSchool = false;

    public List<GameObject> _schoolFishWithLeader = new List<GameObject>();
    public List<SchoolFishBehaviour> _schoolFishWithLeaderBehaviours = new List<SchoolFishBehaviour>();
    private int fishCheckingIndex = 0;
    private int fishCheckingSubdivision = 3;

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
        if (transform.parent.GetComponent<FishZone>())
        {
            _school = transform.parent.GetComponent<FishZone>();
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

        //Correct schoolfish movement
        CorrectSchoolFishMovement();
    }

    private void CorrectSchoolFishMovement()
    {
        if (fishCheckingIndex > fishCheckingSubdivision)
        {
            fishCheckingIndex = 0;
        }

        float count = _schoolFishWithLeader.Count / (fishCheckingSubdivision + 1);
        int startIndex = (int)(count * fishCheckingIndex);

        for (int i = startIndex; i < startIndex + count; i++)
        {
            if (!_schoolFishWithLeaderBehaviours[i].IsFishTooClose())
            {
                for (int j = i + 1; j < startIndex + count; j++)
                {
                    if (Vector3.Distance(_schoolFishWithLeader[i].transform.position, _schoolFishWithLeader[j].transform.position) < _schoolFishWithLeaderBehaviours[j].GetThreatRange())
                    {
                        _schoolFishWithLeaderBehaviours[i].SetFishToAvoid(_schoolFishWithLeaderBehaviours[j]);
                        _schoolFishWithLeaderBehaviours[j].SetFishToAvoid(_schoolFishWithLeaderBehaviours[i]);
                        break;
                    }
                }
            }
        }

        fishCheckingIndex++;
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_checkpoint, 2);
    }
}
