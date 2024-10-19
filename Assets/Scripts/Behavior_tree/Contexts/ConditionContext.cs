using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionContext
{
    public Transform AnyObject { get; }
    public Transform NPC { get; }

    public ConditionContext(Transform anyObject, Transform npc)
    {
        AnyObject = anyObject;
        NPC = npc;
    }
}
