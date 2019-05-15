using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionsManager : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] private float _volume = 0.8f;
    [SerializeField] [Range(0, 15)] private int _maxDistance = 8;

    [HideInInspector] public List<GameObject> CollectableAudioSources = new List<GameObject>();
    [HideInInspector] public Dictionary<string, AudioSource> CollectedAudioSources = new Dictionary<string, AudioSource>();

    void Start()
    {
        SingleTons.CollectionsManager = this;

        GameObject[] gos = GameObject.FindObjectsOfType<GameObject>();
        for (int i = 0; i < gos.Length; i++)
        {
            if (gos[i].layer == 10)
            {
                AudioSource aSource = gos[i].GetComponent<AudioSource>();
                aSource.spatialBlend = 1.0f;
                aSource.volume = _volume;
                aSource.maxDistance = _maxDistance;
                aSource.rolloffMode = AudioRolloffMode.Custom;
                aSource.loop = true;

                if (gos[i].tag == "Collectable")
                    CollectableAudioSources.Add(gos[i]);
            }
        }
    }

    public bool IsCollected(string pGameObjectName)
    {
        foreach (string key in CollectedAudioSources.Keys)
        {
            if (key == pGameObjectName)
                return true;
        }

        return false;
    }

    public void AddToCollection(GameObject pGameObject)
    {
        bool isAlreadyCollected = true;
        for (int i = 0; i < CollectableAudioSources.Count; i++)
        {
            if (CollectableAudioSources[i] == pGameObject)
            {
                CollectedAudioSources.Add(pGameObject.name, pGameObject.GetComponent<AudioSource>());
                isAlreadyCollected = false;
                Debug.Log(string.Format("New audio sample found: {0}", pGameObject.transform.name.ToUpper()));
                break;
            }
        }

        if (isAlreadyCollected)
        {
            print("Object already collected!");
            return;
        }

        for (int i = CollectableAudioSources.Count - 1; i >= 0; i--)
            if (CollectableAudioSources[i].name == pGameObject.name)
                CollectableAudioSources.RemoveAt(i);
    }

    public int GetMaxDistance
    {
        get { return _maxDistance; }
    }
}
