using UnityEditor;
using UnityEngine;

[CreateAssetMenu()]
public class ReconnectCheckpoints : ScriptableObject
{
    [SerializeField] GameObject gameManager;

    public void Reconnect()
    {
        CheckpointSystem.checkpoints = gameManager.transform.Find("Checkpoints").GetComponent<CheckpointSystem>().checkpointFlags;
    }
}
