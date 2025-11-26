using System.Collections;
using Assets.Scripts.Controllers;
using Assets.Scripts.NPC;
using Data;
using Managers;
using UnityEngine;

public class Node_GiveItem : ActionNode
{
    [SerializeField] readonly ItemScriptable item;

    public override string MenuLocation => "Actions/Give Player Item";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        if (PlayerManager.HasRoomInInventory())
            PlayerManager.AddItemToInventory(item);

        yield return null;
    }
}
