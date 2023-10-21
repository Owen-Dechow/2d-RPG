using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    readonly float drag = 0.5f;

    [SerializeField] bool followPlayer;
    private Vector3 shake;

    public static void CenterCameraOnPlayer()
    {
        Camera.main.GetComponent<CameraController>().SetUpMainCamera();
    }

    void SetUpMainCamera()
    {
        if (followPlayer)
        {
            Vector3 position = PlayerController.playerController.transform.position;
            position.z = transform.position.z;
            if (followPlayer) transform.position = position;
        }
    }

    void LateUpdate()
    {
        if (followPlayer)
        {
            Vector3 delta = (PlayerController.playerController.transform.position - transform.position) / drag;
            delta.z = 0;
            transform.position += Time.timeScale * Time.unscaledDeltaTime * delta;
        }
        else
        {
            transform.position = new Vector3(0, 0, transform.position.z);
        }

        transform.position += shake;
    }

    public static IEnumerator ShakeCamera(float time, float intensity, bool decay)
    {
        CameraController cam = Camera.main.GetComponent<CameraController>();
        float startTime = Time.unscaledTime;

        while (Time.unscaledTime - startTime <= time)
        {
            yield return new WaitForSecondsRealtime(0.01f);

            float intensityFactor;
            if (decay)
                intensityFactor = 1 - (Time.unscaledTime - startTime) / time;
            else
                intensityFactor = 1;

            cam.shake = Vector3.zero;
            Vector3 delta = intensityFactor * intensity * new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
            cam.shake += delta;
        }
        cam.shake = Vector3.zero;
    }
}
