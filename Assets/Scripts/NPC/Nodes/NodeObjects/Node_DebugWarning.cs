using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_DebugWarning : ActionNode
{
    public override string ClassName => "console";

    public override string MenuLocation => "Debug/Warning";

    [SerializeField] string message;

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        Debug.LogWarning(message);
        yield return new WaitForEndOfFrame();
    }
}