using System.Collections;
using UnityEngine;

public class Node_AskComplexQuestion : ActionNode
{
    [SerializeField] string prompt;
    [SerializeField] string[] choices;
    [SerializeField] bool allowNoAnswer = false;

    protected override IEnumerator Execute()
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