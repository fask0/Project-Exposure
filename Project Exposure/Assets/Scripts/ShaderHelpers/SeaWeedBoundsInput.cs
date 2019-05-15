using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaWeedBoundsInput : MonoBehaviour
{
    new Renderer renderer;

    [SerializeField]
    private bool _pivotAtBottom = false;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();

        float rand = SingleTons.GameController.GetRandomRange(1, 1000);
        renderer.material.SetFloat("_Offset", rand);

        if (_pivotAtBottom)
        {
            renderer.material.SetFloat("_HighestY", transform.position.y + renderer.bounds.size.y);
            renderer.material.SetFloat("_LowestY", transform.position.y);
        }
        else
        {
            renderer.material.SetFloat("_HighestY", transform.position.y + renderer.bounds.extents.y);
            renderer.material.SetFloat("_LowestY", transform.position.y - renderer.bounds.extents.y);
        }
    }
}
