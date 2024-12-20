using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Controllers
{
    public class Rain : MonoBehaviour
    {
        [SerializeField] bool soundOnly;

        void Start()
        {
            if (soundOnly)
            {
                Destroy(GetComponent<ParticleSystem>());
                transform.Find("Lightning").GetComponent<Lightning>().SoundOnly = true;
            }
        }
    }
}