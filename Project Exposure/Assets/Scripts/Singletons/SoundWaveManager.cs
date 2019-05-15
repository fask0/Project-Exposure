using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class SoundWaveManager : MonoBehaviour
{
    private const int SpectrumSize = 4096;

    //PlayerSoundWave Fields
    private GameObject _playerSoundWave;
    private LineRenderer _playerLineRenderer;
    private float _generalLineWidth;
    private LineRenderer _playerCollectedLineRenderer0;
    private Image _playerCollectedLineRendererBackground0;
    private LineRenderer _playerCollectedLineRenderer1;
    private Image _playerCollectedLineRendererBackground1;
    private LineRenderer _playerCollectedLineRenderer2;
    private Image _playerCollectedLineRendererBackground2;
    private float[] _playerOutputData;
    private float[] _subtractOutput;
    private float[] _individualOutputData;

    //TargetSoundWave Fields
    private GameObject _targetSoundWave;
    private AudioSource _targetAudioSource;
    private LineRenderer _targetLineLineRenderer;
    private float[] _targetOutputData;

    //Scanning Fields
    private GameObject _currentScan;
    private Image _scanProgress;
    private float _scanDuration;
    private float _scanTimeLeft;

    private List<GameObject> _listeningToCollected = new List<GameObject>();
    private List<GameObject> _listeningToAll = new List<GameObject>();

    void Start()
    {
        SingleTons.SoundWaveManager = this;

        initPlayerSoundWave();
        initTargetSoundWave();

        _scanProgress = GameObject.Find("ScanProgress").GetComponent<Image>();
        _scanProgress.enabled = false;
        _scanDuration = 2.0f;
        _scanTimeLeft = _scanDuration;
    }

    void Update()
    {
        UpdatePlayerSoundWave();
        UpdateTargetSoundWave();
    }

    private void initPlayerSoundWave()
    {
        _playerSoundWave = GameObject.Find("PlayerSoundWave");
        _playerLineRenderer = _playerSoundWave.GetComponent<LineRenderer>();
        _playerCollectedLineRenderer0 = _playerLineRenderer.transform.GetChild(0).GetComponent<LineRenderer>();
        _playerCollectedLineRendererBackground0 = _playerCollectedLineRenderer0.GetComponentInChildren<Image>();
        _playerCollectedLineRenderer1 = _playerLineRenderer.transform.GetChild(1).GetComponent<LineRenderer>();
        _playerCollectedLineRendererBackground1 = _playerCollectedLineRenderer1.GetComponentInChildren<Image>();
        _playerCollectedLineRenderer2 = _playerLineRenderer.transform.GetChild(2).GetComponent<LineRenderer>();
        _playerCollectedLineRendererBackground2 = _playerCollectedLineRenderer2.GetComponentInChildren<Image>();
        _playerOutputData = new float[SpectrumSize];
        _individualOutputData = new float[SpectrumSize];
    }

    private void UpdatePlayerSoundWave()
    {
        _playerCollectedLineRenderer0.enabled = false;
        _playerCollectedLineRendererBackground0.enabled = false;
        _playerCollectedLineRenderer1.enabled = false;
        _playerCollectedLineRendererBackground1.enabled = false;
        _playerCollectedLineRenderer2.enabled = false;
        _playerCollectedLineRendererBackground2.enabled = false;
        _subtractOutput = new float[SpectrumSize];

        for (int i = 0; i < _listeningToCollected.Count; i++)
        {
            _listeningToCollected[i].GetComponent<AudioSource>().GetSpectrumData(_individualOutputData, 0, FFTWindow.BlackmanHarris);
            List<float> individualView = new List<float>();
            FillListWithSamples(individualView, _individualOutputData);

            if (i == 0)
            {
                float xScale = 0.0f;
                float xPos = 0.0f;
                if (_listeningToCollected.Count == 1)
                {
                    xScale = 1.0f;
                    xPos = 1.0f;
                }
                else if (_listeningToCollected.Count == 2)
                {
                    xScale = 0.48f;
                    xPos = 0.35f;
                }
                else if (_listeningToCollected.Count == 3)
                {
                    xScale = 0.32f;
                    xPos = 0.15f;
                }

                _playerCollectedLineRenderer0.enabled = true;
                _playerCollectedLineRendererBackground0.enabled = true;
                _playerCollectedLineRenderer0.gameObject.transform.localScale = new Vector3(xScale, _playerCollectedLineRenderer0.gameObject.transform.localScale.y, 1);
                _playerCollectedLineRenderer0.gameObject.transform.localPosition = new Vector3(-(_generalLineWidth * 0.5f) * (1 - xPos), _playerCollectedLineRenderer0.gameObject.transform.localPosition.y, 0);
                SetLinePoints(individualView, _playerCollectedLineRenderer0, Vector3.zero);
            }
            else if (i == 1)
            {
                float xScale = 0.0f;
                float xPos = 0.0f;
                if (_listeningToCollected.Count == 2)
                {
                    xScale = 0.48f;
                    xPos = 0.35f;
                }
                else if (_listeningToCollected.Count == 3)
                {
                    xScale = 0.32f;
                    xPos = 1.0f;
                }

                _playerCollectedLineRenderer1.enabled = true;
                _playerCollectedLineRendererBackground1.enabled = true;
                _playerCollectedLineRenderer1.gameObject.transform.localScale = new Vector3(xScale, _playerCollectedLineRenderer1.gameObject.transform.localScale.y, 1);
                _playerCollectedLineRenderer1.gameObject.transform.localPosition = new Vector3((_generalLineWidth * 0.5f) * (1 - xPos), _playerCollectedLineRenderer1.gameObject.transform.localPosition.y, 0);
                SetLinePoints(individualView, _playerCollectedLineRenderer1, Vector3.zero);
            }
            else if (i == 2)
            {
                float xScale = 0.0f;
                float xPos = 0.0f;
                if (_listeningToCollected.Count == 3)
                {
                    xScale = 0.32f;
                    xPos = 0.15f;
                }

                _playerCollectedLineRenderer2.enabled = true;
                _playerCollectedLineRendererBackground2.enabled = true;
                _playerCollectedLineRenderer2.gameObject.transform.localScale = new Vector3(xScale, _playerCollectedLineRenderer2.gameObject.transform.localScale.y, 1);
                _playerCollectedLineRenderer2.gameObject.transform.localPosition = new Vector3((_generalLineWidth * 0.5f) * (1 - xPos), _playerCollectedLineRenderer2.gameObject.transform.localPosition.y, 0);
                SetLinePoints(individualView, _playerCollectedLineRenderer2, Vector3.zero);
            }

            for (int j = 0; j < SpectrumSize; j++)
            {
                _subtractOutput[j] += _individualOutputData[j];
            }
        }

        AudioListener.GetSpectrumData(_playerOutputData, 0, FFTWindow.BlackmanHarris);
        List<float> playerViewSpectrum = new List<float>();
        FillListWithSamples(playerViewSpectrum, _playerOutputData, _subtractOutput);
        SetLinePoints(playerViewSpectrum, _playerLineRenderer, Vector3.zero);
    }

    private void initTargetSoundWave()
    {
        _targetSoundWave = GameObject.Find("TargetSoundWave");
        _targetSoundWave.transform.localPosition += new Vector3(-625, 0, 0);
        _targetAudioSource = GameObject.Find("TargetSoundDummy").GetComponent<AudioSource>();
        _targetLineLineRenderer = _targetSoundWave.GetComponent<LineRenderer>();
        _targetOutputData = new float[SpectrumSize];
    }

    private void UpdateTargetSoundWave()
    {
        if (_targetAudioSource == null)
        {
            float pointDistance = 0.001f;
            float width = pointDistance * _targetLineLineRenderer.positionCount;

            for (int i = 0; i < _targetLineLineRenderer.positionCount; i++)
                _targetLineLineRenderer.SetPosition(i, new Vector3((-width / 2) + i * pointDistance, 0, 0));

            return;
        }

        _targetAudioSource.GetSpectrumData(_targetOutputData, 0, FFTWindow.BlackmanHarris);
        List<float> targetViewSpectrum = new List<float>();
        FillListWithSamples(targetViewSpectrum, _targetOutputData);
        SetLinePoints(targetViewSpectrum, _targetLineLineRenderer, Vector3.zero);
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

    private void FillListWithSamples(List<float> pList, float[] pSamples, float[] pSubtract = null)
    {
        float bandSize = 1.1f;
        float crossover = bandSize;
        float b = 0.0f;
        for (int i = 0; i < SpectrumSize; i++)
        {
            float d = 0;
            if (pSubtract == null)
                d = pSamples[i];
            else
                d = pSamples[i] - pSubtract[i];
            b = Mathf.Max(d, b);
            if (i > crossover - 3)
            {
                crossover *= bandSize;
                pList.Add(b);
                b = 0;
            }
            if (pSubtract != null)
                pSamples[i] = 0;
        }
    }

    private void SetLinePoints(List<float> pViewSpectrum, LineRenderer pLineRenderer, Vector3 pOffset)
    {
        float pointDistance = 0.001f;
        float width = pointDistance * pViewSpectrum.Count;
        _generalLineWidth = width;

        pLineRenderer.positionCount = pViewSpectrum.Count;
        pLineRenderer.SetPositions(pViewSpectrum.Select((x, i) => new Vector3((-width / 2) + i * pointDistance, (x * 125 < 65) ? x * 150 : 65, 0) + pOffset).ToArray());
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
