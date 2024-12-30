using System.Collections;
using Controllers;
using Managers;
using NPC;
using UnityEngine;

public class Node_ComplexMenu : ActionNode
{
    [SerializeField] string[] choices;
    [SerializeField] bool allowNoAnswer = false;

    public override string MenuLocation => "Menus/Complex Tree";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        yield return GameUIManager.FullMenu(choices, allowNoAnswer);
    }
}
