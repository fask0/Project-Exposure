using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class CollectionsManager : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] private float _volume = 0.8f;
    [SerializeField] [Range(0, 15)] private int _maxDistance = 8;
    [SerializeField] private List<FishScriptableObject> _fishScriptableObjects;

    [HideInInspector] public List<GameObject> collectableAudioSources = new List<GameObject>();
    [HideInInspector] public Dictionary<string, AudioSource> collectedAudioSources = new Dictionary<string, AudioSource>();
    [HideInInspector] public List<GameObject> _allAudioSources = new List<GameObject>();

    private Dictionary<string, Image> _codexMainMenu = new Dictionary<string, Image>();
    private List<Sprite> _fishImages = new List<Sprite>();

    private Sprite _undiscoveredSpeciesSprite;
    private Image _codexSubFishImage;
    private GameObject _codexSubPlayButton;
    private GameObject _codexSubStopButton;
    private AudioSource _codexSubSoundwave;
    private TextMeshProUGUI _codexSubDescription;

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
                _allAudioSources.Add(gos[i]);

                if (gos[i].tag == "Collectable")
                    collectableAudioSources.Add(gos[i]);
            }
        }

        GameObject codexMainMenu = GameObject.Find("CodexMainMenu");
        Image[] fishIcons = codexMainMenu.transform.GetChild(0).GetComponentsInChildren<Image>();
        for (int i = 0; i < fishIcons.Length; i++)
            _codexMainMenu.Add(fishIcons[i].gameObject.name, fishIcons[i]);

        Sprite[] fishSprites = Resources.LoadAll<Sprite>("CodexSprites");
        for (int i = 0; i < fishSprites.Length; i++)
            _fishImages.Add(fishSprites[i]);

        _undiscoveredSpeciesSprite = Resources.Load<Sprite>("CodexSprites/UndiscoveredSpecies");
        GameObject codexSubMenu = GameObject.Find("CodexSubMenu");
        _codexSubFishImage = codexSubMenu.transform.GetChild(0).GetComponent<Image>();
        _codexSubPlayButton = codexSubMenu.transform.GetChild(1).gameObject;
        _codexSubStopButton = codexSubMenu.transform.GetChild(2).gameObject;
        _codexSubStopButton.SetActive(false);
        _codexSubSoundwave = codexSubMenu.transform.GetChild(3).GetComponent<AudioSource>();
        _codexSubDescription = codexSubMenu.transform.GetChild(4).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
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

    public void GotoDescription(GameObject pGameObject)
    {
        _codexSubPlayButton.SetActive(true);
        _codexSubStopButton.SetActive(false);
        for (int i = 0; i < _fishScriptableObjects.Count; i++)
        {
            if (_fishScriptableObjects[i].Name == pGameObject.name)
            {
                if (IsCollected(pGameObject.name))
                {
                    //Discovered Species
                    _codexSubFishImage.sprite = _fishScriptableObjects[i].Sprite;
                    _codexSubSoundwave.clip = _fishScriptableObjects[i].AudioClip;
                    _codexSubDescription.text = _fishScriptableObjects[i].DescriptionFile.text;
                    SingleTons.SoundWaveManager.ResetTexture(_codexSubSoundwave.gameObject.GetComponent<MeshRenderer>().material.mainTexture);
                }
                else
                {
                    //Undiscovered Species
                    _codexSubFishImage.sprite = _undiscoveredSpeciesSprite;
                    _codexSubSoundwave.clip = null;
                    _codexSubDescription.text = "Unknown creature...";
                    SingleTons.SoundWaveManager.ResetTexture(_codexSubSoundwave.gameObject.GetComponent<MeshRenderer>().material.mainTexture);
                }
                return;
            }
        }

        //Undiscovered Species
        _codexSubFishImage.sprite = _undiscoveredSpeciesSprite;
        _codexSubSoundwave.clip = null;
        _codexSubDescription.text = "Unknown creature...";
        SingleTons.SoundWaveManager.ResetTexture(_codexSubSoundwave.gameObject.GetComponent<MeshRenderer>().material.mainTexture);
    }

    public void GotoDescription(string pFishName)
    {
        _codexSubPlayButton.SetActive(true);
        _codexSubStopButton.SetActive(false);
        for (int i = 0; i < _fishScriptableObjects.Count; i++)
        {
            if (_fishScriptableObjects[i].Name == pFishName)
            {
                if (IsCollected(pFishName))
                {
                    //Discovered Species
                    _codexSubFishImage.sprite = _fishScriptableObjects[i].Sprite;
                    _codexSubSoundwave.clip = _fishScriptableObjects[i].AudioClip;
                    _codexSubDescription.text = _fishScriptableObjects[i].DescriptionFile.text;
                    SingleTons.SoundWaveManager.ResetTexture(_codexSubSoundwave.gameObject.GetComponent<MeshRenderer>().material.mainTexture);
                }
                else
                {
                    //Undiscovered Species
                    _codexSubFishImage.sprite = _undiscoveredSpeciesSprite;
                    _codexSubSoundwave.clip = null;
                    _codexSubDescription.text = "Unknown creature...";
                    SingleTons.SoundWaveManager.ResetTexture(_codexSubSoundwave.gameObject.GetComponent<MeshRenderer>().material.mainTexture);
                }
                return;
            }
        }

        //Undiscovered Species
        _codexSubFishImage.sprite = _undiscoveredSpeciesSprite;
        _codexSubSoundwave.clip = null;
        _codexSubDescription.text = "Unknown creature...";
        SingleTons.SoundWaveManager.ResetTexture(_codexSubSoundwave.gameObject.GetComponent<MeshRenderer>().material.mainTexture);
    }

    public void GotoDescriptionFromSpectrogram(List<GameObject> pSpectrogramAudioList, GameObject pButton)
    {
        switch (pButton.transform.parent.parent.name)
        {
            case "0":
                AudioSource collected0 = pSpectrogramAudioList[0].GetComponent<AudioSource>();
                foreach (KeyValuePair<string, AudioSource> entry in collectedAudioSources)
                {
                    if (entry.Value == collected0)
                    {
                        GotoDescription(entry.Key);
                        break;
                    }
                }
                break;

            case "1":
                AudioSource collected1 = pSpectrogramAudioList[1].GetComponent<AudioSource>();
                foreach (KeyValuePair<string, AudioSource> entry in collectedAudioSources)
                {
                    if (entry.Value == collected1)
                    {
                        GotoDescription(entry.Key);
                        break;
                    }
                }
                break;

            case "2":
                AudioSource collected2 = pSpectrogramAudioList[2].GetComponent<AudioSource>();
                foreach (KeyValuePair<string, AudioSource> entry in collectedAudioSources)
                {
                    if (entry.Value == collected2)
                    {
                        GotoDescription(entry.Key);
                        break;
                    }
                }
                break;
        }
    }

    public void ReduceAllSound()
    {
        for (int i = 0; i < _allAudioSources.Count; i++)
            if (_allAudioSources[i] != null)
                _allAudioSources[i].GetComponent<AudioSource>().volume = _volume * 0.1f;
    }

    public void IncreaseAllVolume()
    {
        for (int i = 0; i < _allAudioSources.Count; i++)
            if (_allAudioSources[i] != null)
                _allAudioSources[i].GetComponent<AudioSource>().volume = _volume;
    }

    public void PlayAudioSample()
    {
        if (_codexSubSoundwave.clip != null)
        {
            SingleTons.SoundWaveManager.ResetTexture(_codexSubSoundwave.gameObject.GetComponent<MeshRenderer>().material.mainTexture);
            SingleTons.SoundWaveManager.StartDrawingCustomSpectrogram(_codexSubSoundwave.gameObject, _codexSubSoundwave);
            _codexSubPlayButton.SetActive(false);
            _codexSubStopButton.SetActive(true);
        }
    }

    public void StopAudioSample()
    {
        SingleTons.SoundWaveManager.StopDrawingCustomSpectrogram();
        _codexSubPlayButton.SetActive(true);
        _codexSubStopButton.SetActive(false);
    }
}
