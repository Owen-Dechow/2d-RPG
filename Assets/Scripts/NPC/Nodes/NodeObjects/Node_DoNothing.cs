using System.Collections;
using UnityEngine;

public class Node_DoNothing : ActionNode
{
    public override string Classname => "other";
    protected override IEnumerator Execute()
    {
        yield return null;
    }
}