using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundWaveBehaviour : MonoBehaviour
{
    private GameObject _micArea;
    private Collider _micCollider;
    private List<AudioSource> _audioSourceList = new List<AudioSource>();
    private LineRenderer _lineLinerenderer;
    private float[] _outputData;

    void Start()
    {
        _micArea = GameObject.Find("MicrophoneArea");
        _micCollider = _micArea.transform.GetChild(0).GetComponent<CapsuleCollider>();
        //_audioSourceList.Add(GetComponent<AudioSource>());
        _lineLinerenderer = GetComponent<LineRenderer>();
        _outputData = new float[_lineLinerenderer.positionCount];
        transform.localPosition = new Vector3(-_outputData.Length * 0.0005f, transform.localPosition.y, transform.localPosition.z);
    }

    void Update()
    {
        for (int i = 0; i < _audioSourceList.Count; i++)
        {
            float[] data = new float[_outputData.Length];
            _audioSourceList[i].GetOutputData(data, 1);

            for (int j = 0; j < data.Length; j++)
                _outputData[j] += data[j];
        }

        for (int i = 0; i < _outputData.Length; i++)
        {
            _lineLinerenderer.SetPosition(i, new Vector3(i * 0.001f, _outputData[i] * 5, 0.0f));
            _outputData[i] = 0;
        }
    }

    public void AddAudioSource(AudioSource pAudioSource)
    {
        _audioSourceList.Add(pAudioSource);
    }

    public void RemoveAudioSource(AudioSource pAuidoSource)
    {
        for (int i = 0; i < _audioSourceList.Count; i++)
            if (_audioSourceList[i] == pAuidoSource)
            {
                _audioSourceList[i].volume = 0;
                _audioSourceList.RemoveAt(i);
            }
    }

    public List<AudioSource> AudioList()
    {
        return _audioSourceList;
    }
}
