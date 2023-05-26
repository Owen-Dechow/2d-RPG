using System.Collections;
using UnityEngine;

public class Node_OnInteract : Node
{
    [HideInInspector] public Node child;
    public override string Classname => "root";

    protected override IEnumerator Execute()
    {
        yield return null;
    }

    public override IEnumerator Run()
    {
        if (child != null)
        {
            yield return child.Run();
        }
    }
}
