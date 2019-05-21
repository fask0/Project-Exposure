using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CollectionsManager : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] private float _volume = 0.8f;
    [SerializeField] [Range(0, 15)] private int _maxDistance = 8;

    [HideInInspector] public List<GameObject> collectableAudioSources = new List<GameObject>();
    [HideInInspector] public Dictionary<string, AudioSource> collectedAudioSources = new Dictionary<string, AudioSource>();

    private Dictionary<string, Image> _codexMainMenu = new Dictionary<string, Image>();
    private List<Sprite> _fishImages = new List<Sprite>();

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
                    collectableAudioSources.Add(gos[i]);
            }
        }

        GameObject codexMainMenu = GameObject.Find("CodexMainMenu");
        Image[] fishIcons = codexMainMenu.transform.GetChild(0).GetComponentsInChildren<Image>();
        for (int i = 0; i < fishIcons.Length; i++)
            _codexMainMenu.Add(fishIcons[i].gameObject.name, fishIcons[i]);

        Sprite[] fishSprites = Resources.LoadAll<Sprite>("CodexMainMenu Sprites");
        for (int i = 0; i < fishSprites.Length; i++)
            _fishImages.Add(fishSprites[i]);

        codexMainMenu.transform.parent.gameObject.SetActive(false);
    }

    public bool IsCollected(string pGameObjectName)
    {
        foreach (string key in collectedAudioSources.Keys)
        {
            if (key == pGameObjectName)
                return true;
        }

        return false;
    }

    public void AddToCollection(GameObject pGameObject)
    {
        bool isAlreadyCollected = true;
        for (int i = 0; i < collectableAudioSources.Count; i++)
        {
            if (collectableAudioSources[i] == pGameObject)
            {
                collectedAudioSources.Add(pGameObject.name, pGameObject.GetComponent<AudioSource>());
                isAlreadyCollected = false;
                Debug.Log(string.Format("New audio sample found: {0}", pGameObject.transform.name.ToUpper()));

                foreach (KeyValuePair<string, Image> entry in _codexMainMenu)
                {
                    if (entry.Key == pGameObject.name)
                    {
                        for (int j = 0; j < _fishImages.Count; j++)
                        {
                            if (entry.Key == _fishImages[j].name)
                            {
                                entry.Value.sprite = _fishImages[j];
                                break;
                            }
                        }
                        break;
                    }
                }
                break;
            }
        }

        if (isAlreadyCollected)
        {
            print("Object already collected!");
            return;
        }

        for (int i = collectableAudioSources.Count - 1; i >= 0; i--)
            if (collectableAudioSources[i].name == pGameObject.name)
                collectableAudioSources.RemoveAt(i);
    }

    public int GetMaxDistance
    {
        get { return _maxDistance; }
    }
}
