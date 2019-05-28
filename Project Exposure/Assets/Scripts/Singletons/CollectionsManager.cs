using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class CollectionsManager : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] private float _defaultAudioVolume = 0.8f;
    [SerializeField] [Range(0, 20)] private int _defaultAudioDistanace = 8;
    [SerializeField] private List<FishScriptableObject> _fishScriptableObjects;

    [HideInInspector] public List<GameObject> collectableAudioSources = new List<GameObject>();
    [HideInInspector] public Dictionary<string, AudioSource> collectedAudioSources = new Dictionary<string, AudioSource>();
    [HideInInspector] public List<GameObject> _allAudioSources = new List<GameObject>();

    private List<GameObject> _codexMainMenu = new List<GameObject>();

    private Mesh _undiscoveredSpeciesMesh;
    private Texture _undiscoveredSpeciesTexture;

    private GameObject _codexSubFishModel;
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
                aSource.volume = _defaultAudioVolume;
                aSource.maxDistance = _defaultAudioDistanace;
                aSource.rolloffMode = AudioRolloffMode.Custom;
                aSource.loop = true;
                _allAudioSources.Add(gos[i]);

                if (gos[i].tag == "Collectable")
                    collectableAudioSources.Add(gos[i]);
            }
        }

        GameObject codexMainMenu = Camera.main.transform.GetChild(0).GetChild(7).GetChild(1).gameObject;
        MeshFilter[] fish = codexMainMenu.transform.GetChild(0).GetComponentsInChildren<MeshFilter>();
        for (int i = 0; i < fish.Length; i++)
            _codexMainMenu.Add(fish[i].gameObject);

        GameObject codexSubMenu = Camera.main.transform.GetChild(0).GetChild(7).GetChild(2).gameObject;
        _codexSubFishModel = codexSubMenu.transform.GetChild(1).gameObject;

        _undiscoveredSpeciesMesh = fish[0].gameObject.GetComponent<MeshFilter>().mesh;
        _undiscoveredSpeciesTexture = fish[0].gameObject.GetComponent<MeshRenderer>().material.mainTexture;
        _codexSubFishModel.GetComponent<MeshFilter>().mesh = _undiscoveredSpeciesMesh;
        _codexSubFishModel.GetComponent<MeshRenderer>().material.mainTexture = _undiscoveredSpeciesTexture;

        _codexSubPlayButton = codexSubMenu.transform.GetChild(3).GetChild(1).gameObject;
        _codexSubStopButton = codexSubMenu.transform.GetChild(3).GetChild(2).gameObject;
        _codexSubStopButton.SetActive(false);
        _codexSubSoundwave = codexSubMenu.transform.GetChild(3).GetChild(0).GetChild(1).GetChild(0).GetComponent<AudioSource>();
        SingleTons.SoundWaveManager.ResetTexture(_codexSubSoundwave.gameObject.GetComponent<Image>().material);
        _codexSubDescription = codexSubMenu.transform.GetChild(2).transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        _codexSubDescription.text = "Unknown creature...";
        codexMainMenu.transform.parent.gameObject.SetActive(false);
    }

    private void Update()
    {
        for (int i = 0; i < _codexMainMenu.Count; i++)
        {
            _codexMainMenu[i].transform.rotation *= Quaternion.Euler(0, Time.deltaTime * 100, 0);
        }

        _codexSubFishModel.transform.rotation *= Quaternion.Euler(0, Time.deltaTime * 75, 0);
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

                for (int j = 0; j < _codexMainMenu.Count; j++)
                {
                    if (_codexMainMenu[j].transform.parent.name.ToLower() == pGameObject.name.ToLower())
                    {
                        for (int k = 0; k < _fishScriptableObjects.Count; k++)
                        {
                            if (_codexMainMenu[j].transform.parent.name.ToLower() == _fishScriptableObjects[k].name.ToLower())
                            {
                                _codexMainMenu[j].GetComponent<MeshFilter>().mesh = _fishScriptableObjects[k].Mesh;
                                _codexMainMenu[j].GetComponent<MeshRenderer>().material.mainTexture = _fishScriptableObjects[k].Texture;
                                Vector3 bounds = _codexMainMenu[j].GetComponent<MeshFilter>().mesh.bounds.extents;
                                //_codexMainMenu[j].transform.LookAt(new Vector3(Camera.main.transform.position.x, _codexMainMenu[j].transform.position.y, Camera.main.transform.position.z));
                                //float max = Mathf.Max(Mathf.Max(bounds.x, bounds.y), bounds.z);
                                //_codexMainMenu[j].transform.localScale = new Vector3(1, 1, 1) * max;
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
        get { return _defaultAudioDistanace; }
    }

    public void GotoDescription(GameObject pGameObject)
    {
        StopAudioSample();
        _codexSubFishModel.transform.rotation = Quaternion.Euler(0, 180, 0);
        for (int i = 0; i < _fishScriptableObjects.Count; i++)
        {
            if (_fishScriptableObjects[i].Name.ToLower() == pGameObject.name.ToLower())
            {
                if (IsCollected(pGameObject.name))
                {
                    //Discovered Species
                    _codexSubFishModel.GetComponent<MeshFilter>().mesh = _fishScriptableObjects[i].Mesh;
                    _codexSubFishModel.GetComponent<MeshRenderer>().material.mainTexture = _fishScriptableObjects[i].Texture;
                    _codexSubSoundwave.clip = _fishScriptableObjects[i].AudioClip;
                    _codexSubDescription.text = _fishScriptableObjects[i].DescriptionFile.text;
                    SingleTons.SoundWaveManager.ResetTexture(_codexSubSoundwave.gameObject.GetComponent<Image>().material);
                }
                else
                {
                    //Undiscovered Species
                    _codexSubFishModel.GetComponent<MeshFilter>().mesh = _undiscoveredSpeciesMesh;
                    _codexSubFishModel.GetComponent<MeshRenderer>().material.mainTexture = _undiscoveredSpeciesTexture;
                    _codexSubSoundwave.clip = null;
                    _codexSubDescription.text = "Unknown creature...";
                    SingleTons.SoundWaveManager.ResetTexture(_codexSubSoundwave.gameObject.GetComponent<Image>().material);
                }
                return;
            }
        }

        //Undiscovered Species
        _codexSubFishModel.GetComponent<MeshFilter>().mesh = _undiscoveredSpeciesMesh;
        _codexSubFishModel.GetComponent<MeshRenderer>().material.mainTexture = _undiscoveredSpeciesTexture;
        _codexSubSoundwave.clip = null;
        _codexSubDescription.text = "Unknown creature...";
        SingleTons.SoundWaveManager.ResetTexture(_codexSubSoundwave.gameObject.GetComponent<Image>().material);
    }

    public void GotoDescription(string pFishName)
    {
        StopAudioSample();
        _codexSubFishModel.transform.rotation = Quaternion.Euler(0, 180, 0);
        for (int i = 0; i < _fishScriptableObjects.Count; i++)
        {
            if (_fishScriptableObjects[i].Name == pFishName)
            {
                if (IsCollected(pFishName))
                {
                    //Discovered Species
                    _codexSubFishModel.GetComponent<MeshFilter>().mesh = _fishScriptableObjects[i].Mesh;
                    _codexSubFishModel.GetComponent<MeshRenderer>().material.mainTexture = _fishScriptableObjects[i].Texture;
                    _codexSubSoundwave.clip = _fishScriptableObjects[i].AudioClip;
                    _codexSubDescription.text = _fishScriptableObjects[i].DescriptionFile.text;
                    SingleTons.SoundWaveManager.ResetTexture(_codexSubSoundwave.gameObject.GetComponent<Image>().material);
                }
                else
                {
                    //Undiscovered Species
                    _codexSubFishModel.GetComponent<MeshFilter>().mesh = _undiscoveredSpeciesMesh;
                    _codexSubFishModel.GetComponent<MeshRenderer>().material.mainTexture = _undiscoveredSpeciesTexture;
                    _codexSubSoundwave.clip = null;
                    _codexSubDescription.text = "Unknown creature...";
                    SingleTons.SoundWaveManager.ResetTexture(_codexSubSoundwave.gameObject.GetComponent<Image>().material);
                }
                return;
            }
        }

        //Undiscovered Species
        _codexSubFishModel.GetComponent<MeshFilter>().mesh = _undiscoveredSpeciesMesh;
        _codexSubFishModel.GetComponent<MeshRenderer>().material.mainTexture = _undiscoveredSpeciesTexture;
        _codexSubSoundwave.clip = null;
        _codexSubDescription.text = "Unknown creature...";
        SingleTons.SoundWaveManager.ResetTexture(_codexSubSoundwave.gameObject.GetComponent<Image>().material);
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

        ReduceAllSound();
    }

    public void ReduceAllSound()
    {
        for (int i = 0; i < _allAudioSources.Count; i++)
            if (_allAudioSources[i] != null)
                _allAudioSources[i].GetComponent<AudioSource>().volume = _defaultAudioVolume * 0.1f;
    }

    public void IncreaseAllVolume()
    {
        for (int i = 0; i < _allAudioSources.Count; i++)
            if (_allAudioSources[i] != null)
                _allAudioSources[i].GetComponent<AudioSource>().volume = _defaultAudioVolume;
    }

    public void PlayAudioSample()
    {
        if (_codexSubSoundwave.clip != null)
        {
            SingleTons.SoundWaveManager.ResetTexture(_codexSubSoundwave.gameObject.GetComponent<Image>().material);
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
