using System.Collections;
using Assets.Scripts.Controllers;
using Assets.Scripts.NPC;
using Managers;
using UnityEngine;

public class Node_AskComplexQuestion : ActionNode
{
    [SerializeField] readonly string prompt;
    [SerializeField] readonly string[] choices;
    [SerializeField] readonly bool allowNoAnswer = false;

    public override string MenuLocation => "Menus/Complex Question";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        int cols;
        {
            if (choices.Length <= 3) cols = 1; 
            else if (choices.Length <= 6) cols = 2; 
            else cols = 3;
        }

        yield return GameUIManager.ChoiceMenu(prompt, choices, cols, allowNoAnswer);
    }
}
