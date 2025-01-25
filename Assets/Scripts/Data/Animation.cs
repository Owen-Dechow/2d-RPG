using UnityEngine;

namespace Data
{

    [CreateAssetMenu()]
    public class AnimationScriptable : ScriptableObject
    {
        [SerializeField] private Sprite[] frames;

        public float animationDuration;

        public Sprite GetSprite(float time)
        {
            int frame = Mathf.FloorToInt(time / animationDuration * frames.Length) % frames.Length;
            return frames[frame];
        }
    }

}