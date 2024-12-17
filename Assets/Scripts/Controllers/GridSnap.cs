using UnityEngine;

namespace Controllers
{
    public class GridSnap : MonoBehaviour
    {
        public void SnapTransformToGrid()
        {
            float inter = 0.08f;
            Vector3 positionIDX = (transform.position / inter);

            positionIDX.x = Mathf.Round(positionIDX.x);
            positionIDX.y = Mathf.Round(positionIDX.y);
            positionIDX.z = 0;

            Vector3 newPosition = positionIDX * inter;
            transform.position = newPosition;
        }
    }
}