using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        SingleTons.GameController = this;

        Random.InitState(10);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public float GetRandomRange(float min, float max)
    {
        float rand = UnityEngine.Random.Range(min, max);
        return rand;
    }
}

public static class SingleTons
{
    public static GameController GameController;
    public static FishManager FishManager;
}
