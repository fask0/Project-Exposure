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
        renderer.material.SetFloat("_SeaWeedHeight", (renderer.bounds.size.y / 2) / transform.localScale.y);
    }
}
