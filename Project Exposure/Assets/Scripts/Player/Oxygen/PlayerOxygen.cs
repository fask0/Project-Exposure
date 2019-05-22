using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOxygen : MonoBehaviour
{
    [SerializeField]
    private float _maximumOxygen;
    [SerializeField]
    private float _currentOxygen;

    [SerializeField]
    private float _oxygenDrainRate;
    [SerializeField]
    private GameObject _oxygenContainer;

    private bool _oxygenDrainIsPaused = false;
    private DateTime _resumeTime;

    private float _originalHeight;
    private RectTransform _oxygenContainerRectTransform;

    // Start is called before the first frame update
    void Start()
    {
        _currentOxygen = _maximumOxygen;

        _oxygenContainerRectTransform = _oxygenContainer.GetComponent<RectTransform>();
        _originalHeight = _oxygenContainerRectTransform.rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_oxygenDrainIsPaused)
        {
            _currentOxygen -= _oxygenDrainRate * Time.deltaTime;

            UpdateContainerSize();
            CheckIfDed();
        }
        else
        {
            if (DateTime.Now >= _resumeTime)
            {
                _oxygenDrainIsPaused = false;
            }
        }
    }

    private void CheckIfDed()
    {
        if (_currentOxygen <= 0)
        {
            //DO the ded stuff
        }
    }

    private void UpdateContainerSize()
    {
        float percentage = (_currentOxygen / _maximumOxygen);
        _oxygenContainerRectTransform.offsetMax = new Vector2(_oxygenContainerRectTransform.offsetMax.x, -_originalHeight + (_originalHeight * percentage));
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
