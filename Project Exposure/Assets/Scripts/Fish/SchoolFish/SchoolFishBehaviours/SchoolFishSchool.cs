using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SchoolFishSchool : MonoBehaviour
{
    private List<GameObject> _schoolFish;
    private List<SchoolFishBehaviour> _schoolFishBehaviours;
    private List<GameObject> _otherFish;
    private List<RandomFishBehaviour> _otherFishBehaviours;

    public GameObject _leader;
    public SchoolFishLeaderBehaviour _leaderBehaviour;


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
        _otherFishBehaviours = new List<RandomFishBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AddFishToSchool(GameObject Fish, bool RandomFish = false, bool Leader = false)
    {
        if (RandomFish)
        {
            _otherFishBehaviours.Add(Fish.GetComponent<RandomFishBehaviour>());
            _otherFish.Add(Fish);
            _schoolFish.Add(Fish);
        }
        else
        {
            if (Leader)
            {
                _leaderBehaviour = Fish.GetComponent<SchoolFishLeaderBehaviour>();
                _leader = Fish;
                _schoolFish.Add(Fish);

            }
            else
            {
                _schoolFishBehaviours.Add(Fish.GetComponent<SchoolFishBehaviour>());
                _schoolFish.Add(Fish);
            }
        }
        //}
        //catch
        //{
        //    Debug.Log("Couldn't add Fish... Likely doesn't have the SchoolFishBehaviour");
        //}
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

    public List<GameObject> GetFish() { return _schoolFish; }
    public List<SchoolFishBehaviour> GetFishBehaviours() { return _schoolFishBehaviours; }

    private void OnDrawGizmosSelected()
    {
        if (DrawZoneCube)
        {
            Gizmos.color = new Color(0, 1, 0.8f, 0.5f);
            Gizmos.DrawCube(transform.position, ZoneTransform);
        }
    }
}
