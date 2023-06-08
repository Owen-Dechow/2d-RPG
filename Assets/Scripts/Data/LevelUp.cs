using System.Collections;
using UnityEngine;
using System.Linq;

public class LevelUp : MonoBehaviour
{
    public ExpToLevel[] levelList;
    static LevelUp i;

    private void Start()
    {
        i = this;
    }

    [System.Serializable]
    public struct ExpToLevel
    {
        public int experienceNeeded;
        public int life;
        public int magic;
        public int attack;
        public int defense;
        public int level;
    }

    public static ExpToLevel CheckLevel(int exp)
    {
        return i.levelList.Where(x => x.experienceNeeded <= exp).Last();
    }

    public static int ExpFromLastLevelUp(int exp)
    {
        return exp - i.levelList.Where(x => x.experienceNeeded <= exp).ToArray()[^2].experienceNeeded;
    }
}