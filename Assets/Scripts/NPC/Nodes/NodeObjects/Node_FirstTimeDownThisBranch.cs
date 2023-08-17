using UnityEngine;
using UnityEditor;

public class Node_FirstTimeDownThisBranch : IFNode
{
    public int uniqueId;

    protected override bool Evaluate()
    {
        if (GameManager.NPCActionTreeBranchProtectors.Contains(uniqueId))
        {
            return false;
        }
        else
        {
            GameManager.NPCActionTreeBranchProtectors.Add(uniqueId);
            return true;
        }
    }
}
