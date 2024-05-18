using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_Say : ActionNode
{
    [SerializeField] string text;

    public override string MenuLocation => "Menus/Text Only";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        yield return GameUI.TypeOut(text);
    }
}
