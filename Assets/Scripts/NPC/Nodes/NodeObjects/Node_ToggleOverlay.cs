using System.Collections;
using Assets.Scripts.Controllers;
using Assets.Scripts.NPC;
using Controllers;
using Managers;
using UnityEngine;

public class Node_ToggleOverlay : ActionNode
{
    [SerializeField] readonly bool on = true;
    [SerializeField] readonly float shakePower;
    [SerializeField] readonly bool controlVol = true;
    [SerializeField] readonly bool instant = false;

    public override string MenuLocation => "Animation/Toggle Overlay";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        npc.StartCoroutine(CameraController.ShakeCamera(1, shakePower, false));
        yield return GameUIManager.ToggleLoadingScreen(on, controlVol:controlVol, instant:instant);
    }
}
