using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsCarNearby : ProximityCheck
{
    public bool Check()
    {
        return IsNearbyByTag("Car"); // Возвращает true, если машина рядом
    }

    public override void React()
    {
        Debug.Log("Машина рядом! NPC реагирует соответствующим образом.");
    }
}