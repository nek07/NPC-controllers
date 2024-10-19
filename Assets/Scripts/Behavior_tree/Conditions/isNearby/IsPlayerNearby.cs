using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsPlayerNearby : ProximityCheck
{
    public bool Check()
    {
        return IsNearbyByTag("Player"); // Возвращает true, если игрок рядом
    }

    public override void React()
    {
        Debug.Log("Игрок рядом! NPC реагирует соответствующим образом. SALAMALEYYYYKUUUUM");
    }
}