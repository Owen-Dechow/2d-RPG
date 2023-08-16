using System.Collections;
using UnityEngine;

public class Node_FulfillCheckpoint : ActionNode
{
    [SerializeField] string checkpoint;
    protected override IEnumerator Execute()
    {
        yield return new WaitForEndOfFrame();
        CheckpointSystem.SetCheckpoint(checkpoint);
    }
}