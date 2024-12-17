using System.Collections;
using Controllers;
using Managers;
using NPC;
using UnityEngine;

public class Node_AskYesNoQuestion : ActionNode
{
    [SerializeField] string prompt;
    [SerializeField] string thirdOption;
    [SerializeField] bool allowNoAnswer = false;

    public override string MenuLocation => "Menus/Yes Or No";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        if (thirdOption != null)
        {
            if (thirdOption.Length <= 0) thirdOption = null;
        }
        yield return GameUI.GetYesNo(prompt, thirdOption, allowNoAnswer);
    }
}