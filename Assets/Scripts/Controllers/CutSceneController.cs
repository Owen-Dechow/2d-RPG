using UnityEngine;

namespace Controllers
{
    public class CutSceneController : MonoBehaviour
    {
        void Start()
        {
            Destroy(GetComponent<SpriteRenderer>());
        }
    }
}
