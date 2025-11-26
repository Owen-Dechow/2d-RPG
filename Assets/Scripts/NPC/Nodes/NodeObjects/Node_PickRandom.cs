using Assets.Scripts.Controllers;
using Assets.Scripts.NPC;
using UnityEngine;

public class Node_PickRandom : IFNode
{
    public override string MenuLocation => "Control/Random";

    protected override bool Evaluate(Npc npc, BehaviorTree.TreeData treeData)
    {
        return Random.Range(0, 2) == 1;
    }
}
