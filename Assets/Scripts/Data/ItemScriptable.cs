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

        [SerializeField] int powerMin;
        [SerializeField] int powerMax;
        public int Power => Random.Range(powerMin, powerMax + 1);
    }
}
