using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SoundWaveManager : MonoBehaviour
{
    [SerializeField] private int _heightMultiplier = 300;

    private const int SpectrumSize = 2048;// 4096;

    public delegate void OnFishScan(GameObject pGameObject);
    public event OnFishScan onFishScanEvent;

    ///Fields
    //Spectrogram
    private int _texWidth;
    private int _texHeight;

    //PlayerSoundWave
    //Left
    private GameObject _playerSoundWaveLeft;
    private Material _playerLeftImageMaterial;
    private float[] _playerOutputDataLeft;
    private int _playerLeftColumn;
    //Right
    private GameObject _playerSoundWaveRight;
    private Material _playerRightImageMaterial;
    private int _playerRightColumn;
    private float[] _playerOutputDataRight;
    //CollectedSoundWave
    //0
    private GameObject _collected0;
    private GameObject _collected0Child0;
    private GameObject _collected0Child1;
    private GameObject _collected0Child2;
    private Material _collectedImageMaterial0;
    private int _collected0Column;
    //1
    private GameObject _collected1;
    private GameObject _collected1Child0;
    private GameObject _collected1Child1;
    private Material _collectedImageMaterial1;
    private int _collected1Column;
    //2
    private GameObject _collected2;
    private GameObject _collected2Child0;
    private Material _collectedImageMaterial2;
    private int _collected2Column;
    private float[] _individualOutputData;

    //TargetSoundWave
    private GameObject _targetSoundWave;
    private AudioSource _targetAudioSource;
    private Material _targetImageMaterial;
    private int _targetColumn;
    private float[] _targetOutputData;

    //CursomSoundWave
    private GameObject _customSoundWave;
    private AudioSource _customAudioSource;
    private Material _customImageMaterial;
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

        _scanProgress = Camera.main.transform.GetChild(0).GetChild(2).GetComponent<Image>();
        _scanProgress.enabled = false;
        _scanDuration = 3.0f;
        _scanTimeLeft = _scanDuration;

        _texWidth = _playerLeftImageMaterial.mainTexture.width;
        _texHeight = _playerLeftImageMaterial.mainTexture.height;
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
        _playerSoundWaveLeft = Camera.main.transform.GetChild(0).GetChild(3).gameObject;
        _playerLeftImageMaterial = _playerSoundWaveLeft.transform.GetChild(2).GetChild(0).GetComponent<Image>().material;
        ResetTexture(_playerLeftImageMaterial);
        _playerOutputDataLeft = new float[SpectrumSize];
        //Right
        _playerSoundWaveRight = Camera.main.transform.GetChild(0).GetChild(4).gameObject;
        _playerRightImageMaterial = _playerSoundWaveRight.transform.GetChild(2).GetChild(0).GetComponent<Image>().material;
        ResetTexture(_playerRightImageMaterial);
        _playerOutputDataRight = new float[SpectrumSize];

        //Collected
        GameObject collected = Camera.main.transform.GetChild(0).GetChild(6).gameObject;
        //0
        _collected0 = collected.transform.GetChild(0).gameObject;
        _collectedImageMaterial0 = _collected0.transform.GetChild(0).GetChild(1).GetComponent<Image>().material;
        _collected0Child0 = _collected0.transform.GetChild(0).gameObject;
        _collected0Child0.transform.GetChild(1).GetComponent<Image>().material = _collectedImageMaterial0;
        _collected0Child0.SetActive(false);
        _collected0Child1 = _collected0.transform.GetChild(1).gameObject;
        _collected0Child1.transform.GetChild(1).GetComponent<Image>().material = _collectedImageMaterial0;
        _collected0Child1.SetActive(false);
        _collected0Child2 = _collected0.transform.GetChild(2).gameObject;
        _collected0Child2.transform.GetChild(1).GetComponent<Image>().material = _collectedImageMaterial0;
        _collected0Child2.SetActive(false);
        ResetTexture(_collectedImageMaterial0);
        //1
        _collected1 = collected.transform.GetChild(1).gameObject;
        _collectedImageMaterial1 = _collected1.transform.GetChild(0).GetChild(1).GetComponent<Image>().material;
        _collected1Child0 = _collected1.transform.GetChild(0).gameObject;
        _collected1Child0.transform.GetChild(1).GetComponent<Image>().material = _collectedImageMaterial1;
        _collected1Child0.SetActive(false);
        _collected1Child1 = _collected1.transform.GetChild(1).gameObject;
        _collected1Child1.transform.GetChild(1).GetComponent<Image>().material = _collectedImageMaterial1;
        _collected1Child1.SetActive(false);
        ResetTexture(_collectedImageMaterial1);
        //2
        _collected2 = collected.transform.GetChild(2).gameObject;
        _collectedImageMaterial2 = _collected2.transform.GetChild(0).GetChild(1).GetComponent<Image>().material;
        _collected2Child0 = _collected2.transform.GetChild(0).gameObject;
        _collected2Child0.SetActive(false);
        ResetTexture(_collectedImageMaterial2);

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
                DrawSpectrogram(_collectedImageMaterial0, _individualOutputData, _collected0Column);
                _collected0Column--;
                if (_collected0Column <= 0) _collected0Column = _texWidth - 1;
            }
            else if (i == 1)
            {
                //1
                DrawSpectrogram(_collectedImageMaterial1, _individualOutputData, _collected1Column);
                _collected1Column--;
                if (_collected1Column <= 0) _collected1Column = _texWidth - 1;
            }
            else if (i == 2)
            {
                //2
                DrawSpectrogram(_collectedImageMaterial2, _individualOutputData, _collected2Column);
                _collected2Column--;
                if (_collected2Column <= 0) _collected2Column = _texWidth - 1;
            }

            for (int j = 0; j < SpectrumSize; j++)
                subtractSpecturm[j] += _individualOutputData[j];
        }

        //Update Texture
        AudioListener.GetSpectrumData(_playerOutputDataLeft, 0, FFTWindow.BlackmanHarris);
        DrawSpectrogram(_playerLeftImageMaterial, _playerOutputDataLeft, _playerLeftColumn, subtractSpecturm);
        _playerLeftColumn--;
        if (_playerLeftColumn <= 0) _playerLeftColumn = _texWidth - 1;

        AudioListener.GetSpectrumData(_playerOutputDataRight, 1, FFTWindow.BlackmanHarris);
        DrawSpectrogram(_playerRightImageMaterial, _playerOutputDataRight, _playerRightColumn, subtractSpecturm);
        _playerRightColumn--;
        if (_playerRightColumn <= 0) _playerRightColumn = _texWidth - 1;
    }

    private void InitTargetSoundWave()
    {
        _targetSoundWave = Camera.main.transform.GetChild(0).GetChild(5).gameObject;
        _targetImageMaterial = _targetSoundWave.transform.GetChild(1).GetChild(0).GetComponent<Image>().material;
        ResetTexture(_targetImageMaterial);
        _targetAudioSource = transform.parent.GetChild(5).GetComponent<AudioSource>();
        _targetOutputData = new float[SpectrumSize];
    }

    private void UpdateTargetSoundWave()
    {
        //Update Texture
        _targetAudioSource.GetSpectrumData(_targetOutputData, 0, FFTWindow.BlackmanHarris);
        DrawSpectrogram(_targetImageMaterial, _targetOutputData, _targetColumn);
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
            newTex.SetPixel(x, y, GetGradient(exponentialSpectrum[(int)(y * segmentSize)] * _heightMultiplier, y));
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
    private Color GetGradient(float pValue, float pIndex)
    {
        if (pValue > 0.1f)
            return new Color(1, 1 - (pIndex / _texHeight), 0);
        else
            return new Color(0, 0, 0, 0);
    }

    private void UpdateCustomSoundWave()
    {
        if (!_shouldUpdateCustom) return;

        _customAudioSource.GetSpectrumData(_customOutputData, 0, FFTWindow.BlackmanHarris);
        DrawSpectrogram(_customImageMaterial, _customOutputData, _customColumn);
        _customColumn--;
        if (_customColumn <= 0) _customColumn = _texWidth - 1;
    }

    public void StartDrawingCustomSpectrogram(GameObject pGameObject, AudioSource pAudioSource)
    {
        _customSoundWave = pGameObject;
        _customAudioSource = pAudioSource;
        _customAudioSource.Play();
        _customImageMaterial = _customSoundWave.GetComponent<Image>().material;
        _customImageMaterial.mainTextureOffset = new Vector2(0, -0.0065f);
        _customColumn = 0;
        _customOutputData = new float[SpectrumSize];

        _shouldUpdateCustom = true;
    }

    public void StopDrawingCustomSpectrogram()
    {
        if (_customAudioSource != null)
            _customAudioSource.Stop();
        _shouldUpdateCustom = false;
    }

    public void ResetTexture(Material pMaterial)
    {
        Texture2D tex = pMaterial.mainTexture as Texture2D;
        for (int x = 0; x < tex.width; x++)
            for (int y = 0; y < tex.height; y++)
                tex.SetPixel(x, y, new Color(0, 0, 0, 0));
        tex.Apply();
        pMaterial.mainTexture = tex;
        pMaterial.mainTextureOffset = new Vector2(0, -0.0065f);
    }

    public void ScanCreature(GameObject pScannedCreature)
    {
        Material mat = pScannedCreature.GetComponent<MeshRenderer>().material;
        if (Input.GetKey(KeyCode.Mouse0))
        {
            _currentScan = pScannedCreature;
            _scanTimeLeft -= Time.deltaTime;

            mat.SetFloat("_IsScanning", 1);
            mat.SetFloat("_ScanLines", (_scanDuration - _scanTimeLeft) * 20);
            mat.SetFloat("_ScanLineWidth", _scanDuration - _scanTimeLeft);

            if (_scanTimeLeft <= 0)
            {
                if (onFishScanEvent != null)
                    onFishScanEvent(pScannedCreature);

                SingleTons.CollectionsManager.AddToCollection(pScannedCreature);
                SingleTons.ScoreManager.AddScore(100);

                mat.SetFloat("_IsScanning", 0);
                mat.SetFloat("_ScanLines", 0);
                mat.SetFloat("_ScanLineWidth", 0);

                for (int i = 0; i < _listeningToAll.Count; i++)
                    if (_listeningToAll[i].name == pScannedCreature.name)
                        _listeningToCollected.Add(_listeningToAll[i]);

                _scanTimeLeft = _scanDuration;
            }
        }
        else
        {
            mat.SetFloat("_IsScanning", 0);
            mat.SetFloat("_ScanLines", 0);
            mat.SetFloat("_ScanLineWidth", 0);

            _scanTimeLeft = _scanDuration;
        }

        _scanProgress.fillAmount = (_scanDuration - _scanTimeLeft) / _scanDuration;
    }

    public void ScanTarget(GameObject pScannedTarget)
    {
        Material mat = pScannedTarget.GetComponent<MeshRenderer>().material;
        if (Input.GetKey(KeyCode.Mouse0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 20.0f, ~(1 << 8)))
            {
                if (hit.transform.gameObject == pScannedTarget)
                {
                    int index = 0;
                    int.TryParse(pScannedTarget.tag.Substring(6), out index);
                    if (index < SingleTons.QuestManager.GetCurrentTargetIndex)
                    {
                        Debug.Log("Backtracking!");
                        return;
                    }

                    _currentScan = pScannedTarget;
                    _scanTimeLeft -= Time.deltaTime;

                    mat.SetFloat("_IsScanning", 1);
                    mat.SetFloat("_ScanLines", (_scanDuration - _scanTimeLeft) * 20);
                    mat.SetFloat("_ScanLineWidth", _scanDuration - _scanTimeLeft);

                    if (_scanTimeLeft <= 0)
                    {
                        if (pScannedTarget.tag == string.Format("Target" + SingleTons.QuestManager.GetCurrentTargetIndex))
                        {
                            SingleTons.QuestManager.NextTargetAudio();
                            SingleTons.ScoreManager.AddScore(200);
                        }
                        else if (index > SingleTons.QuestManager.GetCurrentTargetIndex)
                        {
                            for (int i = SingleTons.QuestManager.GetCurrentTargetIndex; i <= index; i++)
                            {
                                SingleTons.ScoreManager.AddScore(200);
                            }
                            SingleTons.QuestManager.SetTargetAudio(index + 1);
                        }

                        mat.SetFloat("_IsScanning", 0);
                        mat.SetFloat("_ScanLines", 0);
                        mat.SetFloat("_ScanLineWidth", 0);

                        _scanTimeLeft = _scanDuration;
                    }
                }
            }
        }
        else
        {
            mat.SetFloat("_IsScanning", 0);
            mat.SetFloat("_ScanLines", 0);
            mat.SetFloat("_ScanLineWidth", 0);

            _scanTimeLeft = _scanDuration;
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

        Material mat = pCurrentScan.GetComponent<MeshRenderer>().material;
        mat.SetFloat("_IsScanning", 0);
        mat.SetFloat("_ScanLines", 0);
        mat.SetFloat("_ScanLineWidth", 0);

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