﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private List<AudioSource> _allTargetsList = new List<AudioSource>();
    private List<AudioSource> _allTargetDummies = new List<AudioSource>();
    private AudioSource _currentAudioTarget;
    private int _currentTargetIndex;

    private AudioSource _soundDummyAudioSource;

    void Start()
    {
        SingleTons.QuestManager = this;
        _soundDummyAudioSource = transform.parent.GetChild(5).GetComponent<AudioSource>();

        _currentTargetIndex = 0;
        for (int i = 0; i < 999; i++)
        {
            GameObject go = GameObject.FindGameObjectWithTag("Target" + i);
            if (go == null) break;

            _allTargetsList.Add(go.GetComponent<AudioSource>());
        }

        SetTargetAudio(_currentTargetIndex);
    }

    public void SetTargetAudio(int pTargetIndex)
    {
        _currentTargetIndex = pTargetIndex;
        if (_currentTargetIndex >= _allTargetsList.Count)
        {
            _currentAudioTarget.Stop();
            _soundDummyAudioSource.Stop();
        }
        else
        {
            _currentAudioTarget = _allTargetsList[_currentTargetIndex].GetComponent<AudioSource>();
            _soundDummyAudioSource.clip = _currentAudioTarget.clip;
            _currentAudioTarget.Stop();
            _soundDummyAudioSource.Play();
            _currentAudioTarget.Play();
        }
    }

    public void NextTargetAudio()
    {
        _currentTargetIndex++;
        if (_currentTargetIndex >= _allTargetsList.Count)
        {
            _currentAudioTarget.Stop();
            _soundDummyAudioSource.Stop();
        }
        else
        {
            _currentAudioTarget = _allTargetsList[_currentTargetIndex].GetComponent<AudioSource>();
            _soundDummyAudioSource.clip = _currentAudioTarget.clip;
            _currentAudioTarget.Stop();
            _soundDummyAudioSource.Play();
            _currentAudioTarget.Play();
        }
    }

    public int GetCurrentTargetIndex
    {
        get { return _currentTargetIndex; }
    }

    public GameObject GetCurrentTarget()
    {
        if (_allTargetsList[_currentTargetIndex] == null) return null;
        return _allTargetsList[_currentTargetIndex].gameObject;
    }
}
