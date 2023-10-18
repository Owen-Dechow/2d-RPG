using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    static CheckpointSystem i;
    public static Checkpoint[] checkpoints;
    public Checkpoint[] checkpointFlags;

    [System.Serializable]
    public class Checkpoint
    {
        public string checkpoint;
        public bool isReached = false;
    }

    [System.Serializable]
    public class CheckpointFlag
    {
        public string name;
    }

    private void Start()
    {
        i = this;
    }

    public static bool GetCheckpoint(string checkpointName)
    {
        checkpointName = checkpointName.ToLower().Replace(' ', '_');
        foreach (Checkpoint checkpoint in i.checkpointFlags)
        {
            if (checkpoint.checkpoint == checkpointName) return checkpoint.isReached;
        }

        Debug.Log($"Unknown Checkpoint: {checkpointName}");
        return false;
    }

    public static void SetCheckpoint(string checkpointName, bool setTo = true)
    {
        checkpointName = checkpointName.ToLower().Replace(' ', '_');
        foreach (Checkpoint checkpoint in i.checkpointFlags)
        {
            if (checkpoint.checkpoint == checkpointName)
            {
                checkpoint.isReached = setTo;
                return;
            }
        }

        Debug.Log($"Unknown Checkpoint: {checkpointName}");
    }

    public static bool GetWindow(string open, string close)
    {
        open = open.ToLower().Replace(' ', '_');
        close = close.ToLower().Replace(' ', '_');

        bool winOpen = open == "" || GetCheckpoint(open);
        if (close != "" && GetCheckpoint(close))
        {
            winOpen = false;
        }
        return winOpen;
    }
};
