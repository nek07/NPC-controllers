using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsDogNearby : ProximityCheck
{
    public bool Check()
    {
        return IsNearbyByTag("Animal"); // Возвращает true, если собака рядом
    }

    public override void React()
    {
        Debug.Log("Собака рядом! NPC реагирует соответствующим образом.");
    }
}