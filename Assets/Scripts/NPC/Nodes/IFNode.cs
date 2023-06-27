using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IFNode : Node
{
    [HideInInspector] public Node @if;
    [HideInInspector] public Node @else;
    public override string ClassName => "if";

    public override IEnumerator Run()
    {
        yield return new WaitForEndOfFrame();
        if (Evaluate())
        {
            if (@if != null) yield return @if.Run();
        }
        else
        {
            if (@else != null) yield return @else.Run();
        }
    }

    protected override IEnumerator Execute()
    {
        yield return null;
    }

    protected abstract bool Evaluate();
}
