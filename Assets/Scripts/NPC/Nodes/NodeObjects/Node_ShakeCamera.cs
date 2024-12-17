using System.Collections;
using Controllers;
using NPC;
using UnityEngine;

public class Node_ShakeCamera : ActionNode
{
    [SerializeField] float time;
    [SerializeField] float intensity;
    [SerializeField] bool decay;

    public override string MenuLocation => "Animation/Shake Camera";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        yield return CameraController.ShakeCamera(time, intensity, decay);
    }
}
