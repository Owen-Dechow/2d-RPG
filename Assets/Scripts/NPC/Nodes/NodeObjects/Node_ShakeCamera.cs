using System.Collections;
using UnityEngine;

public class Node_ShakeCamera : ActionNode
{
    [SerializeField] float time;
    [SerializeField] float intensity;

    protected override IEnumerator Execute(Npc npc)
    {
        yield return CameraController.ShakeCamera(time, intensity);
    }
}
