using Assets.Scripts.Controllers;
using Assets.Scripts.NPC;
using Managers;

public class Node_HasRoomInInventory : IFNode
{
    public override string MenuLocation => "Control/Has Room In Inventory";

    protected override bool Evaluate(Npc npc, BehaviorTree.TreeData treeData)
    {
        return PlayerManager.HasRoomInInventory();
    }
}
