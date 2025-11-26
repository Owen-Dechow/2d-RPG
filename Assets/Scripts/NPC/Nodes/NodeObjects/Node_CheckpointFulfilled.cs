using Assets.Scripts.Controllers;
using Assets.Scripts.NPC;
using Managers;
using UnityEngine;

public class Node_CheckpointFulfilled : IFNode
{
    [SerializeField] readonly CheckpointSystem.CheckpointFlag checkpoint;

    public override string MenuLocation => "Control/Checkpoint Passed";

    protected override bool Evaluate(Npc npc, BehaviorTree.TreeData treeData)
    {
        return checkpoint.GetStatus();
    }
}
