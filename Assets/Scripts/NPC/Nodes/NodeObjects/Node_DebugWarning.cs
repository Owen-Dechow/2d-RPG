using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_DebugWarning : ActionNode
{
    public override string ClassName => "console";
    [SerializeField] string message;

    protected override IEnumerator Execute(Npc npc)
    {
        Debug.LogWarning(message);
        yield return new WaitForEndOfFrame();
    }
}