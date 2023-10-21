using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : MonoBehaviour
{
    [SerializeField] AudioClip[] audioClips;
    [SerializeField] float interval;
    [SerializeField] float shakeIntensity;
    AudioSource thunder;
    ParticleSystem lightning;

    public bool SoundOnly { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        thunder = GetComponent<AudioSource>();
        lightning = GetComponent<ParticleSystem>();
        StartCoroutine(Interval());
    }

    IEnumerator Interval()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(Random.value * interval);

            thunder.clip = audioClips[Random.Range(0, audioClips.Length)];
            thunder.Play();

            StartCoroutine(CameraController.ShakeCamera(thunder.clip.length, shakeIntensity, true));
            
            if (!SoundOnly)
                lightning.Emit(Random.Range(2, 6));

            yield return new WaitWhile(() => thunder.isPlaying);
        }
    }
}
