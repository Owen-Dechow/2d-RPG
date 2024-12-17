using System.Collections;
using Controllers;
using Managers;
using NPC;
using UnityEngine;

public class Node_FulfillCheckpoint : ActionNode
{
    [SerializeField] CheckpointSystem.CheckpointFlag checkpoint;

    public override string MenuLocation => "Actions/Fulfill Checkpoint";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        yield return new WaitForEndOfFrame();
        CheckpointSystem.SetCheckpoint(checkpoint.name);
    }
}