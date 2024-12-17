using UnityEngine;

namespace Managers
{
    public class LevelUp : MonoBehaviour
    {
        public AnimationCurve expNeeded;
        public AnimationCurve life;
        public AnimationCurve magic;
        public AnimationCurve attack;
        public AnimationCurve defense;

        private static LevelUp _i;

        private void Awake()
        {
            _i = this;
        }

        [System.Serializable]
        public struct LevelData
        {
            public int expNeeded;
            public int life;
            public int magic;
            public int attack;
            public int defense;
            public int level;
        }

        public LevelData GetDataForLevel(int level)
        {
            if (level > 100)
                return new LevelData() { expNeeded = int.MaxValue };

            return new LevelData
            {
                level = level,
                life = (int)life.Evaluate(level),
                magic = (int)magic.Evaluate(level),
                attack = (int)attack.Evaluate(level),
                defense = (int)defense.Evaluate(level),
                expNeeded = (int)expNeeded.Evaluate(level),
            };
        }

        public static LevelData GetDataForLevelStatic(int level)
        {
            return _i.GetDataForLevel(level);
        }
    }
}