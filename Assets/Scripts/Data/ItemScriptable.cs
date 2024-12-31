using UnityEngine;

namespace Data
{
    [CreateAssetMenu()]
    public class ItemScriptable : ScriptableObject
    {
        public enum ItemType
        {
            Heal,
            Attack,
            Special
        }

        [SerializeField] private string title;
        public string Title => title;

        [SerializeField] ItemType type;
        public ItemType Type => type;

        [SerializeField] bool canUseOutsideOfBattle;
        public bool CanUseOutsideOfBattle => canUseOutsideOfBattle;

        [SerializeField] Max100 powerMin;
        [SerializeField] Max100 powerMax;
        public Max100 Power => (Max100)Random.Range(powerMin, powerMax + 1);
    }
}
