using System.Collections;
using Controllers;
using UnityEngine;

public class Node_GiveItem : ActionNode
{
    [SerializeField] GameItems.Options item;

    public override string MenuLocation => "Actions/Give Player Item";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        if (Player.HasRoomInInventory())
            Player.AddItemToInventory(item);

        yield return null;
    }
}
