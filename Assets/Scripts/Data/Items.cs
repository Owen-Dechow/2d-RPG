// Ignore Spelling: Scriptable

using System;
using UnityEngine;

public class Items
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
        public String Title { get => itemIdentity.ToString(); }
        public Options itemIdentity;
        public ItemScriptable itemScriptable;
    }

    public static DataSet GetDataForOption(string stringOption)
    {

        stringOption = stringOption.Replace(' ', '_');
        Options option = (Options)Enum.Parse(typeof(Options), stringOption);

        foreach (DataSet dataSet in GameManager.ItemData)
        {
            if (dataSet.itemIdentity == option) return dataSet;
        }

        throw new System.NotImplementedException($"Could not find data for {option}");
    }
}
