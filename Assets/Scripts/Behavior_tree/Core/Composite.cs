using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CompositeNode : Node
{
    protected List<Node> children;

    protected CompositeNode(List<Node> children)
    {
        this.children = children;
    }
}
