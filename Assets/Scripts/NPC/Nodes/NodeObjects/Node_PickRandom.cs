using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_PickRandom : IFNode
{
    public override string MenuLocation => "Control/Random";

    protected override bool Evaluate(Npc npc, BehaviorTree.TreeData treeData)
    {
        return Random.Range(0, 2) == 1;
    }
}