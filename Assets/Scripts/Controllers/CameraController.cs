using UnityEngine;

public class CameraController : MonoBehaviour
{
    readonly float drag = 0.5f;
    [SerializeField] bool followPlayer;
    //[SerializeField] bool snapLocation;
    private Vector3 wantedLocation;

    public static void SetUpCamera()
    {
        Camera.main.GetComponent<CameraController>().SetUpMainCamera();
    }

    void SetUpMainCamera()
    {
        if (followPlayer) transform.position = new Vector3(GameManager.playerLocationLoad.x, GameManager.playerLocationLoad.y, transform.position.z);
        wantedLocation = transform.position;
        SetPosition();
    }

    void Start()
    {
        SetUpMainCamera();
    }

    void LateUpdate()
    {
        if (!followPlayer) return;
        Vector3 delta = (GameManager.player.playerObject.transform.position - wantedLocation) / drag;
        delta.z = 0;
        wantedLocation += Time.timeScale * Time.unscaledDeltaTime * delta;
        SetPosition();
    }

    void SetPosition()
    {
        Vector3 newPosition = wantedLocation;

        //if (false)
        //{
        //    newPosition.x *= 100;
        //    newPosition.x = Mathf.Floor(newPosition.x) / 100;

        //    newPosition.y *= 100;
        //    newPosition.y = Mathf.Floor(newPosition.y) / 100;
        //}

        transform.position = newPosition;
    }
}
