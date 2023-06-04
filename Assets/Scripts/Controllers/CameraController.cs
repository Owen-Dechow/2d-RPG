using UnityEngine;

public class CameraController : MonoBehaviour
{
    readonly float drag = 0.5f;
    [SerializeField] bool followPlayer;

    public static void SetUpCamera()
    {
        Camera.main.GetComponent<CameraController>().SetUpMainCamera();
    }

    void SetUpMainCamera()
    {
        Vector3 postion = GameManager.PlayerPlacementSettings.Position;
        postion.z = transform.position.z;
        if (followPlayer) transform.position = postion;
    }

    void Start()
    {
        SetUpMainCamera();
    }

    void LateUpdate()
    {
        if (!followPlayer) return;
        Vector3 delta = (GameManager.player.playerObject.transform.position - transform.position) / drag;
        delta.z = 0;
        transform.position += Time.timeScale * Time.unscaledDeltaTime * delta;
    }
}
