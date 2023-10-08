using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public static MenuController i;
    private void Start()
    {
        i = this;
    }

    public static IEnumerator Menu()
    {
        Time.timeScale = 0;
        {
            yield return new WaitUntil(() => MyInput.Select == 0);

            List<string> options = new();

            foreach (BattleUnit unit in Player.GetBattleUnits())
            {
                string tag;

                tag = $"{unit.data.title}\\Items\\";
                foreach (GameItems.Options item in unit.data.itemOptionsForUnit)
                {
                    options.Add(tag + GameManager.GetCleanedText(item.ToString()));
                }

                tag = $"{unit.data.title}\\Magic\\";
                foreach (GameMagic.Options magic in unit.data.magicOptionsForUnit)
                {
                    options.Add(tag + GameManager.GetCleanedText(magic.ToString()));
                }
            }

            yield return GameUI.FullMenu(options.ToArray(), true);

            yield return new WaitUntil(() => MyInput.Select == 0);
        }
        Time.timeScale = 1;
    }
}
