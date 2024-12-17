using System.Collections;
using System.Collections.Generic;
using Controllers;
using NPC;
using UnityEngine;

public class Node_GetObject : ActionNode
{
    [SerializeField] string objectName;
    [SerializeField] string referenceKey;

    public override string MenuLocation => "Animation/Get Object";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        GameObject gameObject = GameObject.Find(objectName);
        if (gameObject == null)
        {
            Debug.Log($"Game object of name '{objectName}' cannot be found");
        }
        else
        {
            treeData.GameObjects[referenceKey] = gameObject;
        }

        yield return new WaitForEndOfFrame();
    }
}