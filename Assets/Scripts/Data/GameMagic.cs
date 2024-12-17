// Ignore Spelling: Scriptable

using System;
using Managers;
using UnityEngine;

public class GameMagic : MonoBehaviour
{
    public enum Options
    {
        Sparks,
        Flame_Throw,
        Fire_Dance,
        Rain_Fire,

        Heal_1,
        Heal_2,
        Heal_3,
        Heal_4,
    }

    [System.Serializable]
    public class DataSet
    {
        public string Title { get => identity.ToString(); }
        public Options identity;
        public MagicScriptable scriptable;
    }

    public static DataSet GetDataForOption(string stringOption)
    {
        stringOption = stringOption.Replace(' ', '_');
        Options option = (Options)Enum.Parse(typeof(Options), stringOption);

        foreach (DataSet dataSet in GameManager.MagicData)
        {
            if (dataSet.identity == option) return dataSet;
        }

        throw new System.NotImplementedException($"Could not find data for {option}");
    }
}
