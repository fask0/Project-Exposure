using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAvoider : FishBehaviourParent
{



    private void Start()
    {
        SingleTons.FishManager.AddFishAvoider(this);
    }



}
