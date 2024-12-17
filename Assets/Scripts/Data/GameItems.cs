// Ignore Spelling: Scriptable

using System;
using Managers;
using UnityEngine;

public class GameItems
{
    public enum Options
    {
        Beef_Jerky,
        Bacon,
        Fire_Crackers
    }

    [System.Serializable]
    public class DataSet
    {
        public string Title { get => identity.ToString(); }
        public Options identity;
        public ItemScriptable scriptable;
    }

    public static DataSet GetDataForOption(string stringOption)
    {
        stringOption = stringOption.Replace(' ', '_');
        Options option = (Options)Enum.Parse(typeof(Options), stringOption);

        foreach (DataSet dataSet in GameManager.ItemData)
        {
            if (dataSet.identity == option) return dataSet;
        }

        throw new System.NotImplementedException($"Could not find data for {option}");
    }
}
