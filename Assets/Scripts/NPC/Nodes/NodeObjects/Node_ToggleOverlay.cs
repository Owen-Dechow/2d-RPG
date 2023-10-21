using System.Collections;
using UnityEngine;

public class Node_ToggleOverlay : ActionNode
{
    [SerializeField] bool on = true;
    [SerializeField] float shakePower;
    [SerializeField] bool controlVol = true;
    [SerializeField] bool instant = false;

    protected override IEnumerator Execute(Npc npc)
    {
        npc.StartCoroutine(CameraController.ShakeCamera(1, shakePower, false));
        yield return GameUI.ToggleLoadingScreen(on, controlVol:controlVol, instant:instant);
    }
}
