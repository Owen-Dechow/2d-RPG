using System.Collections;
using UnityEngine;

public class LevelUp : MonoBehaviour
{
    public ExpToLevel[] levelList;

    [System.Serializable]
    public struct ExpToLevel
    {
        public int experienceNeeded;
        public int life;
        public int magic;
        public int attack;
        public int defense;
    }
}