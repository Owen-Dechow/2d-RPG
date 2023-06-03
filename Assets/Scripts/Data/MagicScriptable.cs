// Ignore Spelling: Scriptable

using UnityEngine;

[CreateAssetMenu()]
public class MagicScriptable : ScriptableObject
{
    public enum MagicType
    {
        Heal,
        Attack,
        Special
    }

    [SerializeField] MagicType type;
    public MagicType Type { get => type; }

    [SerializeField] bool canUseOutsideOfBattle;
    public bool CanUseOutsideOfBattle { get => canUseOutsideOfBattle; }

    [SerializeField] int price;
    public int Price { get => price; }

    [SerializeField] int powerMin;
    [SerializeField] int powerMax;
    public int Power { get => Random.Range(powerMin, powerMax + 1); }

    [SerializeField] bool effectsAll;
    public bool EffectsAll { get => effectsAll; }
}
