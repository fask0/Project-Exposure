using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    private List<FishAvoider> fishAvoiders = new List<FishAvoider>();
    private List<FishZone> fishZones = new List<FishZone>();

    // Start is called before the first frame update
    void Start()
    {
        SingleTons.FishManager = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<FishAvoider> GetFishAvoiders()
    {
        return fishAvoiders;
    }

    public void AddFishAvoider(FishAvoider fishAvoider)
    {
        fishAvoiders.Add(fishAvoider);
    }

    public void AddFishZone(FishZone fishZone) { fishZones.Add(fishZone); }
}
