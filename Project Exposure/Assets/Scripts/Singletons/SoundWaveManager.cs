using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWaveManager : MonoBehaviour
{
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

    private float _scanDuration;

    void Start()
    {
        SingleTons.SoundWaveManager = this;
        initPlayerSoundWave();
        initTargetSoundWave();
        _scanDuration = 2.0f;
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
        _playerOutputData = new float[_playerLineRenderer.positionCount];
        _lastFramePlayerOutputData = new float[_playerOutputData.Length];
        _playerSoundWave.transform.localPosition = new Vector3(-_playerOutputData.Length * 0.00125f, _playerSoundWave.transform.localPosition.y, _playerSoundWave.transform.localPosition.z);
    }

    private void UpdatePlayerSoundWave()
    {
        for (int i = 0; i < _playerAudioSourceList.Count; i++)
        {
            float[] data = new float[_playerOutputData.Length];
            _playerAudioSourceList[i].GetOutputData(data, 1);

            for (int j = 0; j < data.Length; j++)
            {
                _playerOutputData[j] += data[j];
                _lastFramePlayerOutputData[j] = 0;
                _lastFramePlayerOutputData[j] += data[j];
            }
        }

        for (int i = 0; i < _playerOutputData.Length; i++)
        {
            _playerLineRenderer.SetPosition(i, new Vector3(i * 0.001f, _playerOutputData[i] * 5, 0.0f));
            _playerOutputData[i] = 0;
        }
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
        _targetOutputData = new float[_targetLineLineRenderer.positionCount];
        _targetSoundWave.transform.localPosition = new Vector3(-_targetOutputData.Length * 0.00125f, _targetSoundWave.transform.localPosition.y, _targetSoundWave.transform.localPosition.z);
    }

    private void UpdateTargetSoundWave()
    {
        if (_targetAudioSource == null)
        {
            for (int i = 0; i < _targetOutputData.Length; i++)
            {
                _targetLineLineRenderer.SetPosition(i, new Vector3(i * 0.001f, 0.0f, 0.0f));
            }
            return;
        }

        _targetAudioSource.GetOutputData(_targetOutputData, 1);

        for (int i = 0; i < _targetOutputData.Length; i++)
        {
            _targetLineLineRenderer.SetPosition(i, new Vector3(i * 0.001f, _targetOutputData[i] * 5, 0.0f));
        }
    }

    public void CompareOutput()
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

        if (Input.GetKey(KeyCode.Mouse0))
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 12.0f, ~(1 << 8)))
            {
                if (hit.transform.tag == string.Format("Target" + SingleTons.QuestManager.GetCurrentTargetIndex))
                {
                    if (accuracy / tries >= 0.75f)
                    {
                        _scanDuration -= Time.deltaTime;
                        if (_scanDuration <= 0)
                        {
                            SingleTons.QuestManager.NextTargetAudio();
                            _scanDuration = 2.0f;
                        }
                    }
                }
            }
        }
    }
}
