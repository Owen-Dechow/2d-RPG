using System.Collections;
using Assets.Scripts.Controllers;
using Assets.Scripts.NPC;
using Controllers;
using UnityEngine;

public class Node_ShakeCamera : ActionNode
{
    [SerializeField] readonly float time;
    [SerializeField] readonly float intensity;
    [SerializeField] readonly bool decay;

    public override string MenuLocation => "Animation/Shake Camera";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        yield return CameraController.ShakeCamera(time, intensity, decay);
    }
}
