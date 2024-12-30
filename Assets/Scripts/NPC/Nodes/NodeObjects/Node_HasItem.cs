using Controllers;
using Data;
using Managers;
using NPC;
using UnityEngine;


public class Node_HasItem : IFNode
{
    [SerializeField] ItemScriptable item;

    public override string MenuLocation => "Control/Has Item";

    protected override bool Evaluate(Npc npc, BehaviorTree.TreeData treeData)
    {
        return PlayerManager.Items.Contains(item);
    }
}
