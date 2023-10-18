using System.Collections;
using UnityEngine;

public class Node_FulfillCheckpoint : ActionNode
{
    [SerializeField] CheckpointSystem.CheckpointFlag checkpoint;
    protected override IEnumerator Execute()
    {
        yield return new WaitForEndOfFrame();
        CheckpointSystem.SetCheckpoint(checkpoint.name);
    }
}