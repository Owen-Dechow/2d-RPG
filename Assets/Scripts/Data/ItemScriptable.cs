// Ignore Spelling: Scriptable

using UnityEngine;

[CreateAssetMenu()]
public class ItemScriptable : ScriptableObject
{
    [SerializeField] bool canUseOutsideOfBattle;
    public bool CanUseOutsideOfBattle { get => canUseOutsideOfBattle; }

    [SerializeField] int attackPowerMin;
    [SerializeField] int attackPowerMax;
    public bool CanAttack { get => attackPowerMin > 0; }
    public int AttackPower { get => Random.Range(attackPowerMin, attackPowerMax + 1); }

    [SerializeField] int healingPowerMin;
    [SerializeField] int healingPowerMax;
    public bool CanHeal { get =>  healingPowerMin > 0; }
    public int HealPower { get => Random.Range(healingPowerMin, healingPowerMax + 1); }
}
