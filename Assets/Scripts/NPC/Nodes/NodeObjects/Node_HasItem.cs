using System.Collections;
using Controllers;
using Managers;
using NPC;
using UnityEngine;


public class Node_HasItem : IFNode
{
    [SerializeField] GameItems.Options item;

    public override string MenuLocation => "Control/Has Item";

    protected override bool Evaluate(Npc npc, BehaviorTree.TreeData treeData)
    {
        return Player.Items.Contains(item);
    }
}
