using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node_Say : ActionNode
{
    [SerializeField] string text;

    protected override IEnumerator Execute()
    {
        yield return GameManager.TypeOut(text);
    }
}
