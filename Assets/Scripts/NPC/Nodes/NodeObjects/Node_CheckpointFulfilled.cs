using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_CheckpointFulfilled : IFNode
{
    [SerializeField] CheckpointSystem.CheckpointFlag checkpoint;

    public override string MenuLocation => "Control/Checkpoint Passed";

    protected override bool Evaluate(Npc npc, BehaviorTree.TreeData treeData)
    {
        return checkpoint.GetStatus();
    }
}
