using System.Collections;
using Controllers;
using Managers;
using NPC;
using UnityEngine;

public class Node_AskComplexQuestion : ActionNode
{
    [SerializeField] string prompt;
    [SerializeField] string[] choices;
    [SerializeField] bool allowNoAnswer = false;

    public override string MenuLocation => "Menus/Complex Question";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        int cols;
        {
            if (choices.Length <= 3) cols = 1; 
            else if (choices.Length <= 6) cols = 2; 
            else cols = 3;
        }

        yield return GameUI.ChoiceMenu(prompt, choices, cols, allowNoAnswer);
    }
}