using UnityEngine;
using UnityEngine.Serialization;

namespace Controllers
{
    public class AudioClipPlayer : MonoBehaviour
    {
        [SerializeField] private AudioClip[] clips;
        private AudioSource audioSource;
        [SerializeField] private float clipCoolDown;


        private void Start() => audioSource = GetComponent<AudioSource>();

        private void Update()
        {
            if (clips.Length <= 0) return;

            if (clipCoolDown <= 0)
            {
                AudioClip clip = clips[Random.Range(0, clips.Length)];
                audioSource.PlayOneShot(clip);
                clipCoolDown = clip.length;
            }
            else
            {
                clipCoolDown -= Time.deltaTime;
            }
        }
    }
}