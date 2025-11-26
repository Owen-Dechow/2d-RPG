using System.Collections;
using Assets.Scripts.Controllers;
using Assets.Scripts.NPC;
using UnityEngine;

public class Node_DebugWarning : ActionNode
{
    public override string ClassName => "console";

    public override string MenuLocation => "Debug/Warning";

    [SerializeField] readonly string message;

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        Debug.LogWarning(message);
        yield return new WaitForEndOfFrame();
    }
}
