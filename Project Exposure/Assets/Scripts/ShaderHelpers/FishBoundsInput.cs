﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishBoundsInput : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)]
    private float _multiplier = 1;

    new Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        var mat = new Material(renderer.sharedMaterial);

        float length = Mathf.Max(renderer.bounds.size.x, renderer.bounds.size.z, renderer.bounds.size.y);
        mat.SetFloat("_FishLength", (length / 2) / transform.localScale.x);

        float rand = SingleTons.GameController.GetRandomRange(0, 1);
        mat.SetFloat("_Offset", rand);

        float speed = transform.parent.GetComponent<FishBehaviour>().GetMinSpeed();
        mat.SetFloat("_WobbleSpeed", speed * 100 * _multiplier);
    }
}
