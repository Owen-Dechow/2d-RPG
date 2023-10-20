using System.Collections;
using UnityEngine;

public class Node_AskYesNoQuestion : ActionNode
{
    [SerializeField] string prompt;
    [SerializeField] string thirdOption;
    [SerializeField] bool allowNoAnswer = false;

    protected override IEnumerator Execute(Npc npc)
    {
        if (thirdOption != null)
        {
            if (thirdOption.Length <= 0) thirdOption = null;
        }
        yield return GameUI.GetYesNo(prompt, thirdOption, allowNoAnswer);
    }
}