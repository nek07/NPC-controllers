using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceNode : CompositeNode
{
    public SequenceNode(List<Node> children) : base(children) { }

    public override NodeState Evaluate(ActionContext actionContext, ConditionContext conditionContext)
    {
        foreach (var child in children)
        {
            NodeState state = child.Evaluate(actionContext, conditionContext); // Pass the separate contexts to each child
            if (state != NodeState.SUCCESS)
            {
                return state; // If any child fails, the sequence returns that state
            }
        }
        return NodeState.SUCCESS; // All children succeeded
    }
}
