using System.Collections;
using UnityEngine;

public class Node_OnInteract : Node
{
    [HideInInspector] public Node child;
    public override string ClassName => "root";

    protected override IEnumerator Execute(Npc npc)
    {
        yield return null;
    }

    public override IEnumerator Run(Npc npc)
    {
        if (child != null)
        {
            yield return child.Run(npc);
        }
    }
}
