using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private List<AudioSource> _allTargetsList = new List<AudioSource>();
    private AudioSource _currentAudioTarget;
    private int _currentTargetIndex;

    private AudioSource _soundDummyAudioSource;

    void Start()
    {
        SingleTons.QuestManager = this;
        _currentTargetIndex = 0;
        _soundDummyAudioSource = GameObject.Find("TargetSoundDummy").GetComponent<AudioSource>();

        for (int i = 0; i < 999; i++)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Target" + i);
            if (go == null) break;

            _allTargetsList.Add(go.GetComponent<AudioSource>());
        }

        SetTargetAudio(0);
    }

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
}
