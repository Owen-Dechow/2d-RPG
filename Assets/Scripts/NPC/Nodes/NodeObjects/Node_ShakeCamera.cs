using System.Collections;
using UnityEngine;

public class Node_ShakeCamera : ActionNode
{
    [SerializeField] float time;
    [SerializeField] float intensity;
    [SerializeField] bool decay;

    protected override IEnumerator Execute(Npc npc)
    {
        yield return CameraController.ShakeCamera(time, intensity, decay);
    }
}
