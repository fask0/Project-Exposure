using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOxygen : MonoBehaviour
{
    [SerializeField]
    private float _maximumOxygen;
    private float _currentOxygen;

    [SerializeField]
    private float _oxygenDrainRate;
    [SerializeField]
    private float _oxygenFillRate;

    private bool _oxygenDrainIsPaused = false;
    private DateTime _resumeTime;

    // Start is called before the first frame update
    void Start()
    {
        _currentOxygen = _maximumOxygen;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_oxygenDrainIsPaused)
        {
            _currentOxygen -= _oxygenDrainRate * Time.deltaTime;
        }
        else
        {
            if (DateTime.Now >= _resumeTime)
            {
                _oxygenDrainIsPaused = false;
            }
        }
    }

    public void PauseOxygenDrain(int pauseTimeInMs)
    {
        _resumeTime = DateTime.Now.AddMilliseconds(pauseTimeInMs);
        _oxygenDrainIsPaused = true;
    }

    public void DrainOxygen(int oxygenAmount)
    {
        _currentOxygen -= oxygenAmount;
    }
}
