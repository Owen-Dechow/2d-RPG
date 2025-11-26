using Assets.Scripts.Controllers;
using Assets.Scripts.NPC;
using Data;
using Managers;
using UnityEngine;


public class Node_HasItem : IFNode
{
    [SerializeField] readonly ItemScriptable item;

    public override string MenuLocation => "Control/Has Item";

    protected override bool Evaluate(Npc npc, BehaviorTree.TreeData treeData)
    {
        return PlayerManager.Items.Contains(item);
    }
}
