using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    static CheckpointSystem i;
    public static Checkpoint[] Checkpoints => i.checkpoint_flags;
    [SerializeField] Checkpoint[] checkpoint_flags;

    [System.Serializable]
    public class Checkpoint
    {
        public string checkpoint;
        public bool isReached = false;
    }

    private void Start()
    {
        i = this;
    }

    public static bool GetCheckpoint(string checkpointName)
    {
        checkpointName = checkpointName.ToLower().Replace(' ', '_');
        foreach (Checkpoint checkpoint in i.checkpoint_flags)
        {
            if (checkpoint.checkpoint == checkpointName) return checkpoint.isReached;
        }

        Debug.Log($"Unknown Checkpoint: {checkpointName}");
        return false;
    }

    public static void SetCheckpoint(string checkpointName, bool setTo = true)
    {
        checkpointName = checkpointName.ToLower().Replace(' ', '_');
        foreach (Checkpoint checkpoint in i.checkpoint_flags)
        {
            if (checkpoint.checkpoint == checkpointName) checkpoint.isReached = setTo;
            return;
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
