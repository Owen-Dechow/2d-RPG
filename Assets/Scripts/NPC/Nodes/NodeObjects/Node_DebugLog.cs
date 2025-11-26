using System.Collections;
using Assets.Scripts.Controllers;
using Assets.Scripts.NPC;
using UnityEngine;

public class Node_DebugLog : ActionNode
{
    public override string ClassName => "console";

    public override string MenuLocation => "Debug/Log";

    [SerializeField] readonly string message;

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        Debug.Log(message);
        yield return new WaitForEndOfFrame();
    }
}
