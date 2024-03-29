﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_DebugError : ActionNode
{
    public override string ClassName => "console";
    [SerializeField] string message;

    protected override IEnumerator Execute(Npc npc)
    {
        Debug.LogException(new System.Exception(message));
        yield return new WaitForEndOfFrame();
    }
}