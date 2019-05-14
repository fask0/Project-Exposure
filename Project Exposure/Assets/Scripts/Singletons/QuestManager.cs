using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private List<AudioSource> _allTargetsList = new List<AudioSource>();
    private AudioSource _currentAudioTarget;
    private int _currentTargetIndex;

    private AudioSource _soundDummyAudioSource;

    //private PlayerSoundWaveBehaviour _playerSoundWaveBehaviour;
    //private TargetSoundWaveBehaviour _targetSoundWaveBehaviour;

    private bool _isListeningToTarget;
    private float _scanDuration;

    void Start()
    {
        SingleTons.QuestManager = this;
        _currentTargetIndex = 0;
        _soundDummyAudioSource = GameObject.Find("TargetSoundDummy").GetComponent<AudioSource>();
        //_playerSoundWaveBehaviour = GameObject.Find("PlayerSoundWave").GetComponent<PlayerSoundWaveBehaviour>();
        //_targetSoundWaveBehaviour = GameObject.Find("TargetSoundWave").GetComponent<TargetSoundWaveBehaviour>();
        //_scanDuration = 1.0f;

        for (int i = 0; i < 999; i++)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Target" + i);
            if (go == null) break;

            _allTargetsList.Add(go.GetComponent<AudioSource>());
        }

        SetTargetAudio(0);
    }

    void Update()
    {
    }

    //public void CompareOutput()
    //{
    //    float accuracy = 0;
    //    float tries = 0;
    //    float[] player = _playerSoundWaveBehaviour.GetOutpt();
    //    float[] target = _targetSoundWaveBehaviour.GetOutpt();
    //    for (int i = 0; i < target.Length; ++i)
    //    {
    //        tries++;
    //        if (target[i] < 0)
    //        {
    //            if (target[i] == _playerSoundWaveBehaviour.GetOutpt()[i])
    //                accuracy++;
    //        }
    //        else if (target[i] > 0)
    //        {
    //            if (target[i] == player[i])
    //                accuracy++;
    //        }
    //    }
    //    print(accuracy);

    //    if (accuracy / tries >= 0.7f)
    //    {
    //        _scanDuration -= Time.deltaTime;
    //        if (_scanDuration <= 0)
    //        {
    //            NextTargetAudio();
    //            print("yay :)");
    //            _scanDuration = 1;
    //        }
    //    }
    //}

    public void SetTargetAudio(int pTargetIndex)
    {
        _currentTargetIndex = pTargetIndex;
        _currentAudioTarget = _allTargetsList[_currentTargetIndex].GetComponent<AudioSource>();
        _soundDummyAudioSource.clip = _currentAudioTarget.clip;
        _currentAudioTarget.Stop();
        _soundDummyAudioSource.Play();
        _currentAudioTarget.Play();
    }

    public void NextTargetAudio()
    {
        _currentTargetIndex++;
        if (_currentTargetIndex >= _allTargetsList.Count)
            _currentTargetIndex = 0;
        _currentAudioTarget = _allTargetsList[_currentTargetIndex].GetComponent<AudioSource>();
        _soundDummyAudioSource.clip = _currentAudioTarget.clip;
        _currentAudioTarget.Stop();
        _soundDummyAudioSource.Play();
        _currentAudioTarget.Play();
    }

    public int GetCurrentTargetIndex
    {
        get { return _currentTargetIndex; }
    }

    public bool IsListeningToTarget
    {
        get { return _isListeningToTarget; }
        set { _isListeningToTarget = value; }
    }
}
