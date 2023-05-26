using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_DebugError : ActionNode
{
    public override string Classname => "console";
    [SerializeField] string message;

    protected override IEnumerator Execute()
    {
        Debug.LogException(new System.Exception(message));
        yield return new WaitForEndOfFrame();
    }
}