using System.Collections;
using Assets.Scripts.Controllers;
using Assets.Scripts.NPC;
using Data;
using Managers;
using UnityEngine;

public class Node_TakeItem : ActionNode
{
    [SerializeField] readonly ItemScriptable item;

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
