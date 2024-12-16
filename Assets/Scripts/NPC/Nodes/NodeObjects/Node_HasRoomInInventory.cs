using System.Collections;
using Controllers;
using UnityEngine;

public class Node_HasRoomInInventory : IFNode
{
    public override string MenuLocation => "Control/Has Room In Inventory";

    protected override bool Evaluate(Npc npc, BehaviorTree.TreeData treeData)
    {
        return Player.HasRoomInInventory();
    }
}
