using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IFNode : Node
{
    [HideInInspector] public Node @if;
    [HideInInspector] public Node @else;
    public override string ClassName => "if";

    public override IEnumerator Run(Npc npc)
    {
        yield return new WaitForEndOfFrame();
        if (Evaluate(npc))
        {
            if (@if != null) yield return @if.Run(npc);
        }
        else
        {
            if (@else != null) yield return @else.Run(npc);
        }
    }

    protected override IEnumerator Execute(Npc npc)
    {
        yield return null;
    }

    protected abstract bool Evaluate(Npc npc);
}
