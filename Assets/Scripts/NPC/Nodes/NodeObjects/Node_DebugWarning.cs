using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_DebugWarning : ActionNode
{
    public override string Classname => "console";
    [SerializeField] string message;

    protected override IEnumerator Execute()
    {
        Debug.LogWarning(message);
        yield return new WaitForEndOfFrame();
    }
}