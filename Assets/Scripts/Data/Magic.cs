using UnityEngine;

public class Magic : MonoBehaviour
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

    public class Data
    {
        public Data(Options option)
        {
            title = GameManager.GetCleanedText(option.ToString());
            switch (option)
            {
                case Options.Sparks:
                    price = 5;
                    attackPower = 8;
                    break;

                case Options.Flame_Throw:
                    price = 10;
                    attackPower = 8;
                    applyAll = true;
                    break;

                case Options.Fire_Dance:
                    price = 15;
                    attackPower = 20;
                    break;

                case Options.Rain_Fire:
                    price = 20;
                    attackPower = 20;
                    applyAll = true;
                    break;

                case Options.Heal_1:
                    price = 5;
                    healingPower = 8;
                    break;

                case Options.Heal_2:
                    price = 10;
                    healingPower = 8;
                    applyAll = true;
                    break;

                case Options.Heal_3:
                    price = 15;
                    healingPower = 20;
                    applyAll = true;
                    break;

                case Options.Heal_4:
                    price = 20;
                    healingPower = 20;
                    applyAll = true;
                    break;
            }
        }

        public string title;

        public int price;
        public bool applyAll = false;
        public bool CanUseOutsideOfBattle { get => healingPower > 0; }
        public bool HasEnoughMagic(BattleUnit unit) => unit.magic >= price;
        
        public int attackPower = 0;
        public bool Attack { get => attackPower > 0; }

        public int healingPower = 0;
        public bool Heal { get => healingPower > 0; }

    }

    public static Data GetDataForOption(string cleanedMagicName)
    {
        System.Enum.TryParse(cleanedMagicName.Replace(' ', '_'), out Options option);
        return new Data(option);
    }
}
