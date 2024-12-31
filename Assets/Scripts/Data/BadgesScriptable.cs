using System;
using UnityEngine;

namespace Data
{
    [CreateAssetMenu()]
    public class BadgesScriptable : ScriptableObject
    {
        public enum BadgeType
        {
            Attack,
            Defense,
            Health,
            Magic,
            Special,
        }

        [SerializeField] private BadgeType type;
        public BadgeType Type => type;

        [SerializeField] private Max100 power;
        public Max100 Power => power;
    }
}