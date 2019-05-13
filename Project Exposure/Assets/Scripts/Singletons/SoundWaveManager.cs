using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class SoundWaveManager : MonoBehaviour
{
    private const int SpectrumSize = 8192;

    //PlayerSoundWave Fields
    private GameObject _playerSoundWave;
    private List<AudioSource> _playerAudioSourceList = new List<AudioSource>();
    private LineRenderer _playerLineRenderer;
    private float[] _playerOutputData;
    private float[] _lastFramePlayerOutputData;

    //TargetSoundWave Fields
    private GameObject _targetSoundWave;
    private AudioSource _targetAudioSource;
    private LineRenderer _targetLineLineRenderer;
    private float[] _targetOutputData;

    //Scanning Fields
    private Image _scanProgress;
    private float _scanDuration;
    private float _scanTimeLeft;

    void Start()
    {
        SingleTons.SoundWaveManager = this;
        initPlayerSoundWave();
        initTargetSoundWave();
        _scanProgress = GameObject.Find("ScanProgress").GetComponent<Image>();
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
        _playerOutputData = new float[SpectrumSize];
        _lastFramePlayerOutputData = new float[SpectrumSize];
    }

    private void UpdatePlayerSoundWave()
    {
        for (int i = 0; i < _playerAudioSourceList.Count; i++)
        {
            float[] data = new float[SpectrumSize];
            _playerAudioSourceList[i].GetSpectrumData(data, 0, FFTWindow.BlackmanHarris);

            for (int j = 0; j < SpectrumSize; j++)
            {
                _playerOutputData[j] += data[j];
                _lastFramePlayerOutputData[j] = 0;
                _lastFramePlayerOutputData[j] += data[j];
            }
        }

        float bandSize = 1.1f;
        float crossover = bandSize;
        List<float> viewSpectrum = new List<float>();
        float b = 0.0f;
        for (int i = 0; i < SpectrumSize; i++)
        {
            var d = _playerOutputData[i];
            b = Mathf.Max(d, b); // find the max as the peak value in that frequency band.
            if (i > crossover)
            {
                crossover *= bandSize; // frequency crossover point for each band..
                viewSpectrum.Add(b);
                b = 0;
            }
            _playerOutputData[i] = 0;
        }

        SetLinePoints(viewSpectrum, _playerLineRenderer, Vector3.zero);
    }

    public void AddAudioSourceToPlayerSoundWave(AudioSource pAudioSource)
    {
        _playerAudioSourceList.Add(pAudioSource);
    }

    public void RemoveAudioSourceFromPlayerSoundWave(AudioSource pAuidoSource)
    {
        for (int i = 0; i < _playerAudioSourceList.Count; i++)
            if (_playerAudioSourceList[i] == pAuidoSource)
            {
                _playerAudioSourceList[i].volume = 0;
                _playerAudioSourceList.RemoveAt(i);
            }
    }

    public List<AudioSource> GetPlayerSoundWaveAudioSourceList
    {
        get { return _playerAudioSourceList; }
    }

    private void initTargetSoundWave()
    {
        _targetSoundWave = GameObject.Find("TargetSoundWave");
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
                _targetLineLineRenderer.SetPosition(i, new Vector3((-width / 2) + i * pointDistance - 0.11f, 0, 0));

            return;
        }

        _targetAudioSource.GetSpectrumData(_targetOutputData, 0, FFTWindow.BlackmanHarris);

        float bandSize = 1.1f;
        float crossover = bandSize;
        List<float> viewSpectrum = new List<float>();
        float b = 0.0f;
        for (int i = 0; i < SpectrumSize; i++)
        {
            var d = _targetOutputData[i];
            b = Mathf.Max(d, b); // find the max as the peak value in that frequency band.
            if (i > crossover)
            {
                crossover *= bandSize; // frequency crossover point for each band..
                viewSpectrum.Add(b);
                b = 0;
            }
        }

        SetLinePoints(viewSpectrum, _targetLineLineRenderer, new Vector3(-0.11f, 0, 0));
    }

    public void CompareOutput()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            float accuracy = 0;
            float tries = 0;
            for (int i = 0; i < _targetOutputData.Length; ++i)
            {
                tries++;
                if (_targetOutputData[i] < 0)
                {
                    if (_targetOutputData[i] >= _lastFramePlayerOutputData[i] * 1.25f)
                        accuracy++;
                }
                else if (_targetOutputData[i] > 0)
                {
                    if (_targetOutputData[i] <= _lastFramePlayerOutputData[i] * 1.25f)
                        accuracy++;
                }
            }

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 12.0f, ~(1 << 8)))
            {
                if (hit.transform.tag == string.Format("Target" + SingleTons.QuestManager.GetCurrentTargetIndex))
                {
                    if (accuracy / tries >= 0.75f)
                    {
                        _scanTimeLeft -= Time.deltaTime;
                        if (_scanTimeLeft <= 0)
                        {
                            SingleTons.QuestManager.NextTargetAudio();
                            _scanTimeLeft = _scanDuration;
                        }
                    }
                }
            }
        }

        _scanProgress.fillAmount = (_scanDuration - _scanTimeLeft) / _scanDuration;
    }

    public void ShowProgress()
    {
        _scanProgress.enabled = true;
    }

    public void HideProgress()
    {
        _scanProgress.enabled = false;
    }

    private void SetLinePoints(List<float> pViewSpectrum, LineRenderer pLineRenderer, Vector3 pOffset)
    {
        float pointDistance = 0.001f;
        float width = pointDistance * pViewSpectrum.Count;

        pLineRenderer.positionCount = pViewSpectrum.Count;
        pLineRenderer.SetPositions(pViewSpectrum.Select((x, i) => new Vector3((-width / 2) + i * pointDistance, x * 34, 0) + pOffset).ToArray());
    }
}
