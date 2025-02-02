// Ignore Spelling: Scriptable

using UnityEngine;

namespace Data
{
    [CreateAssetMenu()]
    public class MagicScriptable : ScriptableObject
    {
        public enum MagicType
        {
            Heal,
            Attack,
            Special
        }

        [SerializeField] private string title;
        public string Title => title;

        [SerializeField] private MagicType type;
        public MagicType Type => type;

        [SerializeField] private bool canUseOutsideOfBattle;
        public bool CanUseOutsideOfBattle => canUseOutsideOfBattle;

        [SerializeField] private int price;
        public int Price => price;

        [SerializeField] private byte powerMin;
        [SerializeField] private byte powerMax;
        public byte Power => (byte)Random.Range(powerMin, powerMax + 1);

        [SerializeField] private bool effectsAll;
        public bool EffectsAll => effectsAll;
    }
}
