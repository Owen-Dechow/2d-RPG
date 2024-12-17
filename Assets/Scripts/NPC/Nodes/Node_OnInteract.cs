using System.Collections;
using Controllers;
using NPC;
using UnityEngine;

public class Node_OnInteract : Node
{
    [HideInInspector] public Node child;
    public override string ClassName => "root";
    public override string MenuLocation => "root";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        yield return null;
    }

    public override IEnumerator Run(Npc npc, BehaviorTree.TreeData treeData)
    {
        if (child != null)
        {
            yield return child.Run(npc, treeData);
        }
    }
}
