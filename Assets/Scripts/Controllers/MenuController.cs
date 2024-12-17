using System.Collections;
using System.Collections.Generic;
using Battle;
using Managers;
using UnityEngine;

namespace Controllers
{
    public class MenuController : MonoBehaviour
    {
        public static IEnumerator Menu()
        {
            using (new CutScene.Window())
            {
                yield return new WaitUntil(() => MyInput.Select == 0);

                List<string> options = new();

                foreach (BattleUnit unit in Player.GetBattleUnits())
                {
                    string tag;

                    if (unit.data.itemOptionsForUnit.Count > 0)
                    {
                        tag = $"{unit.data.title}\\Items\\";
                        foreach (GameItems.Options item in unit.data.itemOptionsForUnit)
                        {
                            options.Add(tag + GameManager.GetCleanedText(item.ToString()));
                        }
                    }
                    else
                    {
                        options.Add($"{unit.data.title}\\Items [empty]");
                    }

                    if (unit.data.magicOptionsForUnit.Count > 0)
                    {
                        tag = $"{unit.data.title}\\Magic\\";
                        foreach (GameMagic.Options magic in unit.data.magicOptionsForUnit)
                        {
                            options.Add(tag + GameManager.GetCleanedText(magic.ToString()));
                        }
                    }
                    else
                    {
                        options.Add($"{unit.data.title}\\Magic [unavailable]");
                    }
                }

                yield return GameUI.FullMenu(options.ToArray(), true);

                yield return new WaitUntil(() => MyInput.Select == 0);
            }
        }
    }
}
