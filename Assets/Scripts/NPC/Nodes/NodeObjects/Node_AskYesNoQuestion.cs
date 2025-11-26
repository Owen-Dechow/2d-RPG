using System.Collections;
using Assets.Scripts.Controllers;
using Assets.Scripts.NPC;
using Managers;
using UnityEngine;

public class Node_AskYesNoQuestion : ActionNode
{
    [SerializeField] readonly string prompt;
    [SerializeField] string thirdOption;
    [SerializeField] readonly bool allowNoAnswer = false;

    public override string MenuLocation => "Menus/Yes Or No";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        if (thirdOption != null)
        {
            if (thirdOption.Length <= 0) thirdOption = null;
        }
        yield return GameUIManager.GetYesNo(prompt, thirdOption, allowNoAnswer);
    }
}
