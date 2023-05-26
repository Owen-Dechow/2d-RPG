using System.Collections;
using UnityEngine;

public abstract class ActionNode : Node
{
    [HideInInspector] public Node child;
    public override string Classname => "action";

    public override IEnumerator Run()
    {
        yield return Execute();
        yield return new WaitForEndOfFrame();
        if (child != null)
        {
            yield return child.Run();
        }
    }

}
