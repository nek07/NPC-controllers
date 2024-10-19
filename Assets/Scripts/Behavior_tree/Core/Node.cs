using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node
{
    public abstract NodeState Evaluate(ActionContext actionContext, ConditionContext conditionContext);
}

public enum NodeState
{
    RUNNING,
    SUCCESS,
    FAILURE
}

public class ActionNode : Node
{
    private System.Func<ActionContext, NodeState> action;

    public ActionNode(System.Func<ActionContext, NodeState> action)
    {
        this.action = action;
    }

    public override NodeState Evaluate(ActionContext actionContext, ConditionContext conditionContext)
    {
        
        return action(actionContext);
    }
}


public class ConditionNode : Node
{
    private System.Func<ConditionContext, bool> condition;

    public ConditionNode(System.Func<ConditionContext, bool> condition)
    {
        this.condition = condition;
    }

    public override NodeState Evaluate(ActionContext actionContext, ConditionContext conditionContext)
    {
            if (conditionContext == null)
    {
        Debug.LogError("ConditionContext is null!");
        return NodeState.FAILURE;
    }

    if (condition == null)
    {
        Debug.LogError("Condition function is null!");
        return NodeState.FAILURE;
    }
        bool conditionResult = condition(conditionContext); // Use the condition function with the provided context
        return conditionResult ? NodeState.SUCCESS : NodeState.FAILURE; // Return success or failure based on the condition result
    }
}

