using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SingleTons.FishManager = this;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
