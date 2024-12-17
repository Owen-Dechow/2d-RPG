using UnityEngine;

namespace Managers
{
    public class CheckpointSystem : MonoBehaviour
    {
        private static CheckpointSystem _i;
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
            public bool None => name == "_";
            public string name = "_";

            public bool GetStatus()
            {
                return GetCheckpoint(name);
            }
        }

        private void Start()
        {
            _i = this;
            checkpoints = checkpointFlags;
        }

        static bool GetCheckpoint(string checkpointName)
        {
            foreach (Checkpoint checkpoint in _i.checkpointFlags)
            {
                if (checkpoint.checkpoint == checkpointName) return checkpoint.isReached;
            }

            return false;
        }

        public static void SetCheckpoint(string checkpointName, bool setTo = true)
        {
            foreach (Checkpoint checkpoint in _i.checkpointFlags)
            {
                if (checkpoint.checkpoint == checkpointName)
                {
                    checkpoint.isReached = setTo;
                    return;
                }
            }
        }

        public static bool GetWindow(CheckpointFlag open, CheckpointFlag close)
        {
            bool winOpen = open.None || open.GetStatus();

            if (!close.None && close.GetStatus())
            {
                winOpen = false;
            }
            return winOpen;
        }
    };
}
