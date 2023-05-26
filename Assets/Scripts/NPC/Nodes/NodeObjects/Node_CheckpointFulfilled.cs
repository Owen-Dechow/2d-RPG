using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_CheckpointFulfilled : IFNode
{
    [SerializeField] string checkpointName;
    protected override bool Eval()
    {
        return GameManager.checkpoints.GetCheckpoint(checkpointName);
    }
}
