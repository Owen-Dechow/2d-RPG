using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items
{
    public enum Options
    {
        Beef_Jerky,
        Bacon,
        Fire_Crackers
    }
    public class Data
    {
        public Data(Options option)
        {
            title = GameManager.GetCleanedText(option.ToString());
            type = option;

            switch (option)
            {
                case Options.Beef_Jerky:
                    healingPower = 5;
                    break;
                case Options.Bacon:
                    healingPower = 7;
                    break;
                case Options.Fire_Crackers:
                    attackPower = 8;
                    break;
            }
        }

        public string title;
        public bool CanUseOutsideOfBattle { get => healingPower > 0; }
        public int attackPower = 0;
        public bool Attack { get => attackPower > 0; }
        public int healingPower = 0;
        public bool Heal { get => healingPower > 0; }
        public Options type;

    }

    public static Data GetDataForOption(string cleanedMagicName)
    {
        System.Enum.TryParse(cleanedMagicName.Replace(' ', '_'), out Options option);

        return new Data(option);
    }
}
