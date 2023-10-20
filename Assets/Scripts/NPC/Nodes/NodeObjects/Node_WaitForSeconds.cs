using System.Collections;
using UnityEngine;

public class Node_WaitForSeconds : ActionNode
{
    public override string ClassName => "other";
    public float seconds;

    protected override IEnumerator Execute(Npc npc)
    {
        if (seconds >= 0) yield return new WaitForSecondsRealtime(seconds);
    }
}