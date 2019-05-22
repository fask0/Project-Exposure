using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaWeedContainer : MonoBehaviour
{
    private List<Renderer> _seaWeedRenderers = new List<Renderer>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            _seaWeedRenderers.Add(transform.GetChild(i).GetComponent<Renderer>());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        foreach (Renderer renderer in _seaWeedRenderers)
        {
            renderer.material.SetVector("_PlayerPos", SingleTons.GameController.Player.transform.position);
        }
    }
}
