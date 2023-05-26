using System.Collections;
using UnityEngine;

public class Node_AskComplexQuestion : ActionNode
{
    [SerializeField] string prompt;
    [SerializeField] string[] choices;
    [SerializeField] bool allowNoAnswer = false;

    protected override IEnumerator Execute()
    {
        yield return GameManager.GetChoice(prompt, choices, allowNoAnswer);
    }
}