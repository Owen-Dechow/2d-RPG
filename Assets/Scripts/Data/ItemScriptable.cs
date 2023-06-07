using UnityEngine;

[CreateAssetMenu()]
public class ItemScriptable : ScriptableObject
{
    public enum ItemType
    {
        Heal,
        Attack,
        Special
    }

    [SerializeField] ItemType type;
    public ItemType Type { get => type; }

    [SerializeField] bool canUseOutsideOfBattle;
    public bool CanUseOutsideOfBattle { get => canUseOutsideOfBattle; }

    [SerializeField] int powerMin;
    [SerializeField] int powerMax;
    public int Power { get => Random.Range(powerMin, powerMax + 1); }
}
