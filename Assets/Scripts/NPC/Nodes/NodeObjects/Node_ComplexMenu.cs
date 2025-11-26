using System.Collections;
using Assets.Scripts.Controllers;
using Assets.Scripts.NPC;
using Managers;
using UnityEngine;

public class Node_ComplexMenu : ActionNode
{
    [SerializeField] readonly string[] choices;
    [SerializeField] readonly bool allowNoAnswer = false;

    public override string MenuLocation => "Menus/Complex Tree";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        yield return GameUIManager.FullMenu(choices, allowNoAnswer);
    }
}
