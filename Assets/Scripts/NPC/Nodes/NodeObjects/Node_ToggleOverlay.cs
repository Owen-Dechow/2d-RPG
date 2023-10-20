using System.Collections;
using UnityEngine;

public class Node_ToggleOverlay : ActionNode
{
    [SerializeField] bool on;
    [SerializeField] float shakePower;

    protected override IEnumerator Execute(Npc npc)
    {
        npc.StartCoroutine(CameraController.ShakeCamera(1, shakePower));
        yield return GameUI.ToggleLoadingScreen(on);
    }
}
