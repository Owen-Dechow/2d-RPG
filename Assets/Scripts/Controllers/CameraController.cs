using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Controllers
{
    public class CameraController : MonoBehaviour
    {
        private readonly int materialColorField = Shader.PropertyToID("_Color");
        private const float Drag = 0.5f;

        [SerializeField] public Material sceneIndependentSpriteMaterial;
        [SerializeField] public Color multiplier;

        [SerializeField] private bool followPlayer;
        private Vector3 shake;

        public static void CenterCameraOnPlayer()
        {
            Camera.main?.GetComponent<CameraController>().SetUpMainCamera();
        }

        void SetUpMainCamera()
        {
            sceneIndependentSpriteMaterial.SetColor(materialColorField, multiplier);

            if (!followPlayer)
                return;
            
            Vector3 position = PlayerController.playerController.transform.position;
            position.z = transform.position.z;
            
            if (followPlayer)
                transform.position = position;
        }

        private void LateUpdate()
        {
            if (followPlayer)
            {
                Vector3 delta = (PlayerController.playerController.transform.position - transform.position) / Drag;
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
            CameraController cam = Camera.main!.GetComponent<CameraController>();
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
                Vector3 delta = intensityFactor * intensity *
                                new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
                cam.shake += delta;
            }

            cam.shake = Vector3.zero;
        }

        private void OnDrawGizmos()
        {
            sceneIndependentSpriteMaterial.SetColor(materialColorField, multiplier);
        }
    }
}