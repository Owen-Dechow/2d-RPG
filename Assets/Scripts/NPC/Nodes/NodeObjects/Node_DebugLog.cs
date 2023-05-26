using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_DebugLog : ActionNode
{
    public override string Classname => "console";
    [SerializeField] string message;

    protected override IEnumerator Execute()
    {
        Debug.Log(message);
        yield return new WaitForEndOfFrame();
    }
}
