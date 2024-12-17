using System.Collections;
using System.Collections.Generic;
using Controllers;
using NPC;
using UnityEngine;

public abstract class Node : ScriptableObject
{
    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 position;
    [HideInInspector] public abstract string ClassName { get; }
    public abstract string MenuLocation { get; }

    public abstract IEnumerator Run(Npc npc, BehaviorTree.TreeData treeData);
    protected abstract IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData);
}
