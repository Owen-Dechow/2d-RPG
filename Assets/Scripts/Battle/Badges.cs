using UnityEngine;

public class Badges : MonoBehaviour
{
    public enum Defense
    {
        NONE,

        Moss,
        Paper,
        Cherry,
        Oak,
        Marble,
        Garnet,
        Iron,
        Copper,
        Dimond,

        Fire,
        Wind,
        Earth,
        Water,

        Sight,
        Smell,
        Hearing,
        Feeling,
        Taste,

        Life,
        Death,

        Time,
        Null,
        Magic,
        Reality,
        Power,
    }
    public enum Attack {
        NONE,

        Moss,
        Paper,
        Cherry,
        Oak,
        Marble,
        Garnet,
        Iron,
        Copper,
        Dimond,

        Fire,
        Wind,
        Earth,
        Water,

        Sight,
        Smell,
        Hearing,
        Feeling,
        Taste,

        Life,
        Death,

        Time,
        Null,
        Magic,
        Reality,
        Power,
    }
    public enum Special
    {
        NONE,
        
        Moss,
        Paper, 
        Cherry, 
        Oak, 
        Marble, 
        Garnet, 
        Iron, 
        Copper, 
        Dimond,

        Fire, 
        Wind, 
        Earth, 
        Water,

        Sight, 
        Smell, 
        Hearing, 
        Feeling, 
        Taste, 

        Life, 
        Death,

        Time, 
        Null,
        Magic, 
        Reality,
        Power,
    }

    public enum ExtraBadgeKind
    {
        Attack,
        Defence,
    }

    [System.Serializable]
    public class Equip
    {
        public Defense defense;
        public Attack attack;
        public Special special;

        public ExtraBadgeKind extra1Kind;
        public ExtraBadgeKind extra2Kind;
        public Attack extraAttack1;
        public Attack extraAttack2;
        public Defense extraDefense1;
        public Defense extraDefense2;

        private const float attackUpper = 100;
        private const float attackNoise = 20;
        private const float defenseUpper = 50;
        private const float numberOfBadges = 25;

        public int GetDefenseChange(int startAttack, bool onDefence)
        {
            float defenseBadge = (int)defense / numberOfBadges;
            float extraBadge1 = (extra1Kind == ExtraBadgeKind.Defence) ? ((int)extraDefense1 / numberOfBadges) : 0;
            float extraBadge2 = (extra2Kind == ExtraBadgeKind.Defence) ? ((int)extraDefense2 / numberOfBadges) : 0;
            float percent = defenseBadge + extraBadge1 + extraBadge2;
            
            float newAttack = startAttack - (defenseUpper * percent);
            if (onDefence) newAttack *= 0.7f;
            int finalAttack = (int)Mathf.Clamp(newAttack, 1, int.MaxValue);
            return finalAttack;
        }

        public int GetAttack()
        {
            float attackBadge = (int)attack / numberOfBadges;
            float extraBadge1 = (extra1Kind == ExtraBadgeKind.Attack) ? ((int)extraAttack1 / numberOfBadges) : 0;
            float extraBadge2 = (extra2Kind == ExtraBadgeKind.Attack) ? ((int)extraAttack2 / numberOfBadges) : 0;
            float percent = attackBadge + extraBadge1 + extraBadge2;

            float preRandomizedAttack = Mathf.Clamp(percent * attackUpper, 1, int.MaxValue);
            int noise = Mathf.RoundToInt(percent * attackNoise) + 1;
            float newAttack = preRandomizedAttack + Random.Range(-noise, noise + 1);
            int finalAttack = (int)Mathf.Clamp(newAttack, 1, int.MaxValue);
            return finalAttack;
        }
    }

    [System.Serializable]
    public class Inventory
    {
        public Defense[] defense;
        public Attack[] attack;
        public Special[] special;
    }
}