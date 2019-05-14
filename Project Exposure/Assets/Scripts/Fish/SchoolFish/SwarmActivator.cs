using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmActivator : MonoBehaviour
{
    [SerializeField]
    private SwarmableArea _swarmArea;
    [SerializeField]
    private FishZone _fishZone;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F11))
        {
            _swarmArea.SwarmArea(_fishZone);
        }
        if (Input.GetKeyDown(KeyCode.F12))
        {
            _swarmArea.StopSwarming();
        }
    }
}
