using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRadar : MonoBehaviour
{
    [SerializeField]
    private GameObject _objToActivate;
    [SerializeField]
    private int _activateAfterSeconds;

    private DateTime _activationTime = DateTime.MaxValue;
    private bool _hasBeenActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        _activationTime = DateTime.Now.AddSeconds(_activateAfterSeconds);
        SingleTons.SoundWaveManager.onFishScanEvent += ResetRadar;
    }

    private void ResetRadar(GameObject pGameObject)
    {
        if (pGameObject.tag.Contains("Target"))
        {
            _activationTime = DateTime.Now.AddSeconds(_activateAfterSeconds);
            _objToActivate.SetActive(false);
            _hasBeenActivated = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (DateTime.Now > _activationTime)
        {
            if (!_hasBeenActivated)
            {
                _objToActivate.SetActive(true);
                _hasBeenActivated = true;
            }

            transform.LookAt(SingleTons.QuestManager.GetCurrentTarget().transform);
        }
    }
}
