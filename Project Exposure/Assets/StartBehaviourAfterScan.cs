using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBehaviourAfterScan : MonoBehaviour
{
    private FishBehaviour fishBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        fishBehaviour = GetComponent<FishBehaviour>();
        SingleTons.SoundWaveManager.onFishScanEvent += StartFishBehaviour;
    }

    private void OnDisable()
    {
        SingleTons.SoundWaveManager.onFishScanEvent -= StartFishBehaviour;
    }

    void StartFishBehaviour(GameObject obj)
    {
        if (obj.name == this.name)
        {
            fishBehaviour.enabled = true;
        }
        else
        {
            Debug.Log(obj.name);
        }
    }
}
