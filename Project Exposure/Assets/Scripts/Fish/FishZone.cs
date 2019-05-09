using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishZone : MonoBehaviour
{
    private List<GameObject> _fishToAvoid;
    private List<FishBehaviour> _fishToAvoidBehaviours;
    private List<GameObject> _otherFish;

    //School fish stuff
    private List<GameObject> _schoolFish;
    private List<SchoolFishBehaviour> _schoolFishBehaviours;

    private List<GameObject> _leaders;
    private List<SchoolFishLeaderBehaviour> _leaderBehaviours;
    private int _leaderIndex = 0;

    [SerializeField]
    private Vector3 ZoneTransform = new Vector3();
    [SerializeField]
    private bool DrawZoneCube = true;

    // Start is called before the first frame update
    void Start()
    {
        _schoolFish = new List<GameObject>();
        _schoolFishBehaviours = new List<SchoolFishBehaviour>();

        _otherFish = new List<GameObject>();
        _fishToAvoid = new List<GameObject>();
        _fishToAvoidBehaviours = new List<FishBehaviour>();

        _leaders = new List<GameObject>();
        _leaderBehaviours = new List<SchoolFishLeaderBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddFishToSchool(GameObject Fish, bool RandomFish = false, bool Leader = false)
    {
        if (!RandomFish)
        {
            if (Leader)
            {
                _leaderBehaviours.Add(Fish.GetComponent<SchoolFishLeaderBehaviour>());
                _leaders.Add(Fish);
                _schoolFish.Add(Fish);
            }
            else
            {
                _schoolFishBehaviours.Add(Fish.GetComponent<SchoolFishBehaviour>());
                _schoolFish.Add(Fish);
                _schoolFishBehaviours[_schoolFishBehaviours.Count - 1].SetSchoolFishLeader(_leaderBehaviours[_leaderIndex]);
                _leaderIndex++;
                if (_leaderIndex >= _leaders.Count) { _leaderIndex = 0; }
            }
        }
        _fishToAvoid.Add(Fish);
        _fishToAvoidBehaviours.Add(Fish.GetComponent<FishBehaviour>());
    }

    public Vector3 GenerateNewCheckPoint(Vector3 fishPos)
    {
        Vector3 checkPoint = fishPos - transform.position;
        while (Vector3.Distance(checkPoint + transform.position, fishPos) < 10)
        {
            float randomX = UnityEngine.Random.Range(-Mathf.Abs(ZoneTransform.x) / 2, Mathf.Abs(ZoneTransform.x) / 2);
            float randomY = UnityEngine.Random.Range(-Mathf.Abs(ZoneTransform.y) / 2, Mathf.Abs(ZoneTransform.y) / 2);
            float randomZ = UnityEngine.Random.Range(-Mathf.Abs(ZoneTransform.z) / 2, Mathf.Abs(ZoneTransform.z) / 2);
            checkPoint = new Vector3(randomX, randomY, randomZ);
        }

        return (checkPoint + transform.position);
    }

    public List<GameObject> GetSchoolFish() { return _schoolFish; }
    public List<GameObject> GetFishToAvoid() { return _fishToAvoid; }
    public List<FishBehaviour> GetFishToAvoidBehaviours() { return _fishToAvoidBehaviours; }
    public List<SchoolFishBehaviour> GetSchoolFishBehaviours() { return _schoolFishBehaviours; }

    private void OnDrawGizmosSelected()
    {
        if (DrawZoneCube)
        {
            Gizmos.color = new Color(0, 1, 0.8f, 0.5f);
            Gizmos.DrawCube(transform.position, ZoneTransform);
        }
    }
}
