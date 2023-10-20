using System.Collections;
using UnityEngine;

public abstract class ActionNode : Node
{
    [HideInInspector] public Node child;
    public override string ClassName => "action";

    public override IEnumerator Run(Npc npc)
    {
        yield return Execute(npc);
        yield return new WaitForEndOfFrame();
        if (child != null)
        {
            yield return child.Run(npc);
        }
    }
}
