﻿using System.Collections;
using UnityEngine;

public class Node_ComplexMenu : ActionNode
{
    [SerializeField] string[] choices;
    [SerializeField] bool allowNoAnswer = false;

    protected override IEnumerator Execute()
    {
        yield return GameUI.FullMenu(choices, allowNoAnswer);
    }
}
