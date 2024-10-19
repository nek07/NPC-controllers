using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class SelectorNode : CompositeNode
{
    public SelectorNode(List<Node> children) : base(children) { }

    public override NodeState Evaluate(ActionContext actionContext, ConditionContext conditionContext)
    {
        foreach (var child in children)
        {
            NodeState state = child.Evaluate(actionContext, conditionContext); // Pass the separate contexts to each child
            if (state == NodeState.SUCCESS)
            {
                return NodeState.SUCCESS; // If any child succeeds, the selector returns success
            }
        }
        return NodeState.FAILURE; // All children failed
    }
}

