using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_Say : ActionNode
{
    [SerializeField] string text;

    protected override IEnumerator Execute(Npc npc)
    {
        yield return GameUI.TypeOut(text);
    }
}
