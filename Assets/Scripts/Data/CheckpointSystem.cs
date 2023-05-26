using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    public Checkpoint[] checkpoints;

    [System.Serializable]
    public class Checkpoint
    {
        public string checkpoint;
        public bool isReached = false;
    }

    public bool GetCheckpoint(string checkpointName)
    {
        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (checkpoint.checkpoint == checkpointName) return checkpoint.isReached;
        }

        Debug.Log($"Unknown Checkpoint: {checkpointName}");
        return false;
    }

    public void SetCheckpoint(string checkpointName, bool setTo = true)
    {
        foreach (Checkpoint checkpoint in checkpoints)
        {
            if (checkpoint.checkpoint == checkpointName) checkpoint.isReached = setTo;
            return;
        }

        Debug.Log($"Unknown Checkpoint: {checkpointName}");
    }
};
