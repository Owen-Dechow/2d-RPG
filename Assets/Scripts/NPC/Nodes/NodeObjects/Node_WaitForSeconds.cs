using System.Collections;
using Assets.Scripts.Controllers;
using Assets.Scripts.NPC;
using UnityEngine;

public class Node_WaitForSeconds : ActionNode
{
    public override string ClassName => "other";

    public override string MenuLocation => "Animation/Wait For Second";

    public float seconds;

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        if (seconds >= 0) yield return new WaitForSecondsRealtime(seconds);
    }
}
