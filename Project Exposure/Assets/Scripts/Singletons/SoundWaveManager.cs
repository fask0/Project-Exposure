using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Profiling;

public class SoundWaveManager : MonoBehaviour
{
    [SerializeField] private int _heightMultiplier = 300;

    private const int SpectrumSize = 4096;

    ///Fields
    //Spectrogram
    private int _texWidth;
    private int _texHeight;

    //PlayerSoundWave
    //Left
    private GameObject _playerSoundWaveLeft;
    private Material _playerLeftQuadMat;
    private float[] _playerOutputDataLeft;
    private int _playerLeftColumn;
    //Right
    private GameObject _playerSoundWaveRight;
    private Material _playerRightQuadMat;
    private int _playerRightColumn;
    private float[] _playerOutputDataRight;
    //CollectedSoundWave
    //0
    private GameObject _collected0;
    private GameObject _collected0Child0;
    private GameObject _collected0Child1;
    private GameObject _collected0Child2;
    private Material _collectedQuadMat0;
    private int _collected0Column;
    //1
    private GameObject _collected1;
    private GameObject _collected1Child0;
    private GameObject _collected1Child1;
    private Material _collectedQuadMat1;
    private int _collected1Column;
    //2
    private GameObject _collected2;
    private GameObject _collected2Child0;
    private Material _collectedQuadMat2;
    private int _collected2Column;
    private float[] _individualOutputData;

    //TargetSoundWave
    private GameObject _targetSoundWave;
    private AudioSource _targetAudioSource;
    private Material _targetQuadMat;
    private int _targetColumn;
    private float[] _targetOutputData;

    //CursomSoundWave
    private GameObject _customSoundWave;
    private AudioSource _customAudioSource;
    private Material _customMaterial;
    private int _customColumn;
    private float[] _customOutputData;
    private bool _shouldUpdateCustom;

    //Scanning
    private GameObject _currentScan;
    private Image _scanProgress;
    private float _scanDuration;
    private float _scanTimeLeft;

    private List<GameObject> _listeningToCollected = new List<GameObject>();
    private List<GameObject> _listeningToAll = new List<GameObject>();

    void Start()
    {
        SingleTons.SoundWaveManager = this;

        InitPlayerSoundWave();
        InitTargetSoundWave();

        _scanProgress = GameObject.Find("ScanProgress").GetComponent<Image>();
        _scanProgress.enabled = false;
        _scanDuration = 2.0f;
        _scanTimeLeft = _scanDuration;

        _texWidth = _playerLeftQuadMat.mainTexture.width;
        _texHeight = _playerLeftQuadMat.mainTexture.height;
        _playerLeftColumn = 0;
    }

    void Update()
    {
        UpdatePlayerSoundWave();
        UpdateTargetSoundWave();
        UpdateCustomSoundWave();
    }

    private void InitPlayerSoundWave()
    {
        //Left
        _playerSoundWaveLeft = GameObject.Find("PlayerSoundWaveLeft");
        _playerLeftQuadMat = _playerSoundWaveLeft.transform.GetChild(0).GetComponent<MeshRenderer>().material;
        ResetTexture(_playerLeftQuadMat.mainTexture);
        _playerOutputDataLeft = new float[SpectrumSize];
        //Right
        _playerSoundWaveRight = GameObject.Find("PlayerSoundWaveRight");
        _playerRightQuadMat = _playerSoundWaveRight.transform.GetChild(0).GetComponent<MeshRenderer>().material;
        ResetTexture(_playerRightQuadMat.mainTexture);
        _playerOutputDataRight = new float[SpectrumSize];

        //Collected
        GameObject collected = GameObject.Find("Collected");
        //0
        _collected0 = collected.transform.GetChild(0).gameObject;
        _collectedQuadMat0 = _collected0.transform.GetChild(0).GetComponent<MeshRenderer>().material;
        _collected0Child0 = _collected0.transform.GetChild(0).gameObject;
        _collected0Child0.GetComponent<MeshRenderer>().material = _collectedQuadMat0;
        _collected0Child0.SetActive(false);
        _collected0Child1 = _collected0.transform.GetChild(1).gameObject;
        _collected0Child1.GetComponent<MeshRenderer>().material = _collectedQuadMat0;
        _collected0Child1.SetActive(false);
        _collected0Child2 = _collected0.transform.GetChild(2).gameObject;
        _collected0Child2.GetComponent<MeshRenderer>().material = _collectedQuadMat0;
        _collected0Child2.SetActive(false);
        ResetTexture(_collectedQuadMat0.mainTexture);
        //1
        _collected1 = collected.transform.GetChild(1).gameObject;
        _collectedQuadMat1 = _collected1.transform.GetChild(0).GetComponent<MeshRenderer>().material;
        _collected1Child0 = _collected1.transform.GetChild(0).gameObject;
        _collected1Child0.GetComponent<MeshRenderer>().material = _collectedQuadMat1;
        _collected1Child0.SetActive(false);
        _collected1Child1 = _collected1.transform.GetChild(1).gameObject;
        _collected1Child1.GetComponent<MeshRenderer>().material = _collectedQuadMat1;
        _collected1Child1.SetActive(false);
        ResetTexture(_collectedQuadMat1.mainTexture);
        //2
        _collected2 = collected.transform.GetChild(2).gameObject;
        _collectedQuadMat2 = _collected2.transform.GetChild(0).GetComponent<MeshRenderer>().material;
        _collected2Child0 = _collected2.transform.GetChild(0).gameObject;
        _collected2Child0.SetActive(false);
        ResetTexture(_collectedQuadMat2.mainTexture);

        _individualOutputData = new float[SpectrumSize];
    }

    private void UpdatePlayerSoundWave()
    {
        if (_listeningToCollected.Count != 0)
        {
            if (_listeningToCollected.Count == 1)
            {
                //0
                _collected0Child0.SetActive(true);
                _collected0Child1.SetActive(false);
                _collected0Child2.SetActive(false);
                //1
                _collected1Child0.SetActive(false);
                _collected1Child1.SetActive(false);
                //2
                _collected2Child0.SetActive(false);
            }
            else if (_listeningToCollected.Count == 2)
            {
                //0
                _collected0Child0.SetActive(false);
                _collected0Child1.SetActive(true);
                _collected0Child2.SetActive(false);
                //1
                _collected1Child0.SetActive(true);
                _collected1Child1.SetActive(false);
                //2
                _collected2Child0.SetActive(false);
            }
            else if (_listeningToCollected.Count >= 3)
            {
                //0
                _collected0Child0.SetActive(false);
                _collected0Child1.SetActive(false);
                _collected0Child2.SetActive(true);
                //1
                _collected1Child0.SetActive(false);
                _collected1Child1.SetActive(true);
                //2
                _collected2Child0.SetActive(true);
            }
        }
        else
        {
            //0
            _collected0Child0.SetActive(false);
            _collected0Child1.SetActive(false);
            _collected0Child2.SetActive(false);
            //1
            _collected1Child0.SetActive(false);
            _collected1Child1.SetActive(false);
            //2
            _collected2Child0.SetActive(false);
        }

        //Update Collected
        float[] subtractSpecturm = new float[SpectrumSize];
        for (int i = 0; i < _listeningToCollected.Count; i++)
        {
            _listeningToCollected[i].GetComponent<AudioSource>().GetSpectrumData(_individualOutputData, 0, FFTWindow.BlackmanHarris);

            if (i == 0)
            {
                //0
                DrawSpectrogram(_collectedQuadMat0, _individualOutputData, _collected0Column);
                _collected0Column--;
                if (_collected0Column <= 0) _collected0Column = _texWidth - 1;
            }
            else if (i == 1)
            {
                //1
                DrawSpectrogram(_collectedQuadMat1, _individualOutputData, _collected1Column);
                _collected1Column--;
                if (_collected1Column <= 0) _collected1Column = _texWidth - 1;
            }
            else if (i == 2)
            {
                //2
                DrawSpectrogram(_collectedQuadMat2, _individualOutputData, _collected2Column);
                _collected2Column--;
                if (_collected2Column <= 0) _collected2Column = _texWidth - 1;
            }

            for (int j = 0; j < SpectrumSize; j++)
                subtractSpecturm[j] += _individualOutputData[j];
        }

        //Update Texture
        AudioListener.GetSpectrumData(_playerOutputDataLeft, 0, FFTWindow.BlackmanHarris);
        DrawSpectrogram(_playerLeftQuadMat, _playerOutputDataLeft, _playerLeftColumn, subtractSpecturm);
        _playerLeftColumn--;
        if (_playerLeftColumn <= 0) _playerLeftColumn = _texWidth - 1;

        AudioListener.GetSpectrumData(_playerOutputDataRight, 1, FFTWindow.BlackmanHarris);
        DrawSpectrogram(_playerRightQuadMat, _playerOutputDataRight, _playerRightColumn, subtractSpecturm);
        _playerRightColumn--;
        if (_playerRightColumn <= 0) _playerRightColumn = _texWidth - 1;
    }

    private void InitTargetSoundWave()
    {
        _targetSoundWave = GameObject.Find("TargetSoundWave");
        _targetQuadMat = _targetSoundWave.transform.GetChild(0).GetComponent<MeshRenderer>().material;
        ResetTexture(_targetQuadMat.mainTexture);
        _targetAudioSource = GameObject.Find("TargetSoundDummy").GetComponent<AudioSource>();
        _targetOutputData = new float[SpectrumSize];
    }

    private void UpdateTargetSoundWave()
    {
        //Update Texture
        _targetAudioSource.GetSpectrumData(_targetOutputData, 0, FFTWindow.BlackmanHarris);
        DrawSpectrogram(_targetQuadMat, _targetOutputData, _targetColumn);
        _targetColumn--;
        if (_targetColumn <= 0) _targetColumn = _texWidth - 1;
    }

    /// <summary>
    /// asd
    /// </summary>
    /// <param name="pMesh">Original mesh</param>
    /// <param name="pMaterial">Original material</param>
    /// <param name="pSpectrum">Linear spectrum</param>
    private void DrawSpectrogram(Material pMaterial, float[] pSpectrum, int pColumn, float[] pSubtract = null)
    {
        // Transfer from Linear spectrum to an Exponential one:
        float bandSize = 1.1f;
        float crossover = bandSize;
        float b = 0.0f;
        List<float> exponentialSpectrum = new List<float>();
        for (int i = 0; i < SpectrumSize; i++)
        {
            float d = 0.0f;

            if (pSubtract == null)
                d = pSpectrum[i];
            else
                d = pSpectrum[i] - pSubtract[i];

            b = Mathf.Max(d, b);
            if (i > crossover)
            {
                crossover *= bandSize;
                exponentialSpectrum.Add(b);
                b = 0;
            }
        }

        // Every pixel represents this many data points from the spectrum:
        float segmentSize = (float)exponentialSpectrum.Count / (float)_texHeight;

        // Draw the pixels and apply them to the original texture:
        Texture2D newTex = pMaterial.mainTexture as Texture2D;
        for (int y = 0; y < _texHeight; y++)
        {
            int x = _texWidth - 1 - pColumn;
            newTex.SetPixel(x, y, GetGradient(exponentialSpectrum[(int)(y * segmentSize)] * _heightMultiplier));
        }
        newTex.Apply();
        pMaterial.mainTexture = newTex;

        // Offset the texture:
        pMaterial.mainTextureOffset += new Vector2(1 / ((float)_texWidth - 1), 0);
    }

    /// <summary>
    /// Returns a color for each value between 0 and 1, chosen from a smooth gradient
    /// </summary>
    /// <param name="pValue"></param>
    /// <returns></returns>
    private Color GetGradient(float pValue)
    {
        return new Color(pValue * 10, pValue, 0);
    }

    private void UpdateCustomSoundWave()
    {
        if (!_shouldUpdateCustom) return;

        _customAudioSource.GetSpectrumData(_customOutputData, 0, FFTWindow.BlackmanHarris);
        DrawSpectrogram(_customMaterial, _customOutputData, _customColumn);
        _customColumn--;
        if (_customColumn <= 0) _customColumn = _texWidth - 1;
    }

    public void StartDrawingCustomSpectrogram(GameObject pGameObject, AudioSource pAudioSource)
    {
        _customSoundWave = pGameObject;
        _customAudioSource = pAudioSource;
        _customAudioSource.Play();
        _customMaterial = _customSoundWave.GetComponent<MeshRenderer>().material;
        _customMaterial.mainTextureOffset = new Vector2(0, 0);
        _customColumn = 0;
        _customOutputData = new float[SpectrumSize];

        _shouldUpdateCustom = true;
    }

    public void StopDrawingCustomSpectrogram()
    {
        _customAudioSource.Stop();
        _shouldUpdateCustom = false;
    }

    public void ResetTexture(Texture pTexture)
    {
        Texture2D tex = pTexture as Texture2D;
        for (int x = 0; x < tex.width; x++)
            for (int y = 0; y < tex.height; y++)
                tex.SetPixel(x, y, Color.black);
        tex.Apply();
        pTexture = tex;
    }

    public void ScanObject(GameObject pScannedObject)
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 12.0f, ~(1 << 8)))
            {
                if (hit.transform.tag == string.Format("Target" + SingleTons.QuestManager.GetCurrentTargetIndex))
                {
                    _currentScan = pScannedObject;
                    _scanTimeLeft -= Time.deltaTime;
                    if (_scanTimeLeft <= 0)
                    {
                        SingleTons.QuestManager.NextTargetAudio();
                        _scanTimeLeft = _scanDuration;
                    }
                }
                else if (hit.transform.tag == "Collectable")
                {
                    if (pScannedObject != hit.transform.gameObject) return;
                    _currentScan = pScannedObject;
                    _scanTimeLeft -= Time.deltaTime;
                    if (_scanTimeLeft <= 0)
                    {
                        SingleTons.CollectionsManager.AddToCollection(hit.transform.gameObject);

                        for (int i = 0; i < _listeningToAll.Count; i++)
                            if (_listeningToAll[i].name == pScannedObject.name)
                                _listeningToCollected.Add(_listeningToAll[i]);

                        _scanTimeLeft = _scanDuration;
                    }
                }
            }
        }

        _scanProgress.fillAmount = (_scanDuration - _scanTimeLeft) / _scanDuration;
    }

    public void ShowProgress(GameObject pCurrentScan)
    {
        if (_currentScan == null || _currentScan != pCurrentScan) return;

        _scanProgress.enabled = true;
    }

    public void HideProgress(GameObject pCurrentScan)
    {
        if (_currentScan == null || _currentScan != pCurrentScan) return;

        _scanTimeLeft = _scanDuration;
        _scanProgress.enabled = false;
    }

    public void AddSource(GameObject pGameObject)
    {
        _listeningToCollected.Add(pGameObject);
    }

    public List<GameObject> GetListeningToCollected
    {
        get { return _listeningToCollected; }
    }

    public List<GameObject> GetListeningToAll
    {
        get { return _listeningToAll; }
    }
}