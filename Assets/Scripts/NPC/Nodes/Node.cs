using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Node : ScriptableObject
{
    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 position;
    [HideInInspector] public abstract string ClassName { get; }

    public abstract IEnumerator Run();
    protected abstract IEnumerator Execute();
}
