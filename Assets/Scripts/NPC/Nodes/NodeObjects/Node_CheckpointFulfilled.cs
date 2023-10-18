using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_CheckpointFulfilled : IFNode
{
    [SerializeField] CheckpointSystem.CheckpointFlag checkpoint;
    protected override bool Evaluate()
    {
        return CheckpointSystem.GetCheckpoint(checkpoint.name);
    }
}
