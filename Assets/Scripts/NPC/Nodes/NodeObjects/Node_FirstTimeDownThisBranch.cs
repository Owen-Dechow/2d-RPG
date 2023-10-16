using UnityEngine;
using UnityEditor;

public class Node_FirstTimeDownThisBranch : IFNode
{
    public int uniqueId;

    protected override bool Evaluate()
    {
        if (GameManager.PostInteractionProtectionIDs.Contains(uniqueId))
        {
            return false;
        }
        else
        {
            GameManager.PostInteractionProtectionIDs.Add(uniqueId);
            return true;
        }
    }
}
