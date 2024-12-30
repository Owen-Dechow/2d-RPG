using System.Collections;
using Controllers;
using Data;
using Managers;
using NPC;
using UnityEngine;

public class Node_GiveItem : ActionNode
{
    [SerializeField] ItemScriptable item;

    public override string MenuLocation => "Actions/Give Player Item";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        if (PlayerManager.HasRoomInInventory())
            PlayerManager.AddItemToInventory(item);

        yield return null;
    }
}
