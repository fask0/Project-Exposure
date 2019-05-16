using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaWeedBoundsInput : MonoBehaviour
{
    new Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();

        float rand = SingleTons.GameController.GetRandomRange(1, 1000);
        renderer.material.SetFloat("_Offset", rand);


        renderer.material.SetFloat("_HighestY", renderer.bounds.center.y + renderer.bounds.extents.y);
        renderer.material.SetFloat("_LowestY", renderer.bounds.center.y - renderer.bounds.extents.y);
    }
}
