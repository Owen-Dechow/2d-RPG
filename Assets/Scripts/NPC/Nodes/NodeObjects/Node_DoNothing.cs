using System.Collections;
using UnityEngine;

public class Node_DoNothing : ActionNode
{
    public override string ClassName => "other";
    protected override IEnumerator Execute(Npc npc)
    {
        yield return null;
    }
}