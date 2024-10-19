using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ActionContext
{
    public UnityEngine.AI.NavMeshAgent Agent { get; }
    public Animator Animator { get; }
    public Transform AnyObject { get; }
    public Transform[] Destination { get; }

    public ActionContext(UnityEngine.AI.NavMeshAgent agent, Animator animator, Transform anyObject, Transform[] destination)
    {
        Agent = agent;
        Animator = animator;
        AnyObject = anyObject;
        Destination = destination;
    }
}