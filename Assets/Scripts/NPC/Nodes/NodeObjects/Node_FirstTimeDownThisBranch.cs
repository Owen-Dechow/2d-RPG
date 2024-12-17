﻿using Controllers;
using Managers;
using NPC;
using UnityEngine;
using UnityEditor;

public class Node_FirstTimeDownThisBranch : IFNode
{
    public int uniqueId;

    public override string MenuLocation => "Control/First Time At This Node";

    protected override bool Evaluate(Npc npc, BehaviorTree.TreeData treeData)
    {
        if (GameManager.postInteractionProtectionIDs.Contains(uniqueId))
        {
            return false;
        }
        else
        {
            GameManager.postInteractionProtectionIDs.Add(uniqueId);
            return true;
        }
    }
}
