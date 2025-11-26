using System.Collections;
using Assets.Scripts.Controllers;
using Assets.Scripts.NPC;
using Managers;
using UnityEngine;

public class Node_FulfillCheckpoint : ActionNode
{
    [SerializeField] readonly CheckpointSystem.CheckpointFlag checkpoint;

    public override string MenuLocation => "Actions/Fulfill Checkpoint";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        yield return new WaitForEndOfFrame();
        CheckpointSystem.SetCheckpoint(checkpoint.name);
    }
}
