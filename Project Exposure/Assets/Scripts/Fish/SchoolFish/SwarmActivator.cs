using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwarmActivator : MonoBehaviour
{
    [SerializeField]
    private SwarmableArea _swarmArea;
    [SerializeField]
    private FishZone _fishZone;

    private enum ActivationShapeRange
    {
        Sphere,
        Box
    };

    [SerializeField]
    private ActivationShapeRange activationShape = ActivationShapeRange.Box;

    [SerializeField]
    private float _fishDisperseRange;
    [SerializeField]
    private float _fishSwarmRange;

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

        float diff = (transform.position - SingleTons.GameController.Player.transform.position).magnitude;
        if (diff * diff < _fishDisperseRange * _fishDisperseRange)
        {
            _swarmArea.StopSwarming();
        }
        else if (diff * diff < _fishSwarmRange * _fishSwarmRange)
        {
            _swarmArea.SwarmArea(_fishZone);
        }
        else
        {
            _swarmArea.StopSwarming();
        }
    }

    private void OnDrawGizmosSelected()
    {
        switch (activationShape)
        {
            case ActivationShapeRange.Sphere:
                Gizmos.color = new Color(255, 165, 0);
                Gizmos.DrawWireSphere(transform.position, _fishDisperseRange);
                Gizmos.color = new Color(128, 0, 128);
                Gizmos.DrawWireSphere(transform.position, _fishSwarmRange);
                break;
            case ActivationShapeRange.Box:
                break;
        }
    }
}
