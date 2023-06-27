using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_CheckpointFulfilled : IFNode
{
    [SerializeField] string checkpointName;
    protected override bool Evaluate()
    {
        return GameManager.checkpoints.GetCheckpoint(checkpointName);
    }
}
