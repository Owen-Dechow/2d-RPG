using System.Collections;
using Controllers;
using Managers;
using NPC;
using UnityEngine;

public class Node_ToggleOverlay : ActionNode
{
    [SerializeField] bool on = true;
    [SerializeField] float shakePower;
    [SerializeField] bool controlVol = true;
    [SerializeField] bool instant = false;

    public override string MenuLocation => "Animation/Toggle Overlay";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {
        npc.StartCoroutine(CameraController.ShakeCamera(1, shakePower, false));
        yield return GameUI.ToggleLoadingScreen(on, controlVol:controlVol, instant:instant);
    }
}
