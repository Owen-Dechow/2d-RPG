using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;

public class Node_DebugLog : ActionNode
{
    public override string ClassName => "console";

    public override string MenuLocation => "Debug/Log";

    [SerializeField] string message;

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        Debug.Log(message);
        yield return new WaitForEndOfFrame();
    }
}
