using System.Collections;
using Controllers;
using Data;
using Managers;
using NPC;
using UnityEngine;

public class Node_TakeItem : ActionNode
{
    [SerializeField] ItemScriptable item;

    public override string MenuLocation => "Actions/Take Player Item";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        if (PlayerManager.Items.Contains(item))
        {
            PlayerManager.Items.Remove(item);
        }
        yield return null;
    }
}
