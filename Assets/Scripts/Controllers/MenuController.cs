using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Battle;
using Data;
using Managers;
using Managers.CutScene;
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

                foreach (BattleUnit unit in PlayerManager.GetBattleUnits())
                {
                    string tag;

                    if (unit.data.itemOptionsForUnit.Count > 0)
                    {
                        tag = $"{unit.data.title}\\Items\\";
                        options.AddRange(unit.data.itemOptionsForUnit.Select(item =>
                            tag + GameUIManager.GetCleanedText(item.ToString())));
                    }
                    else
                    {
                        options.Add($"{unit.data.title}\\Items [empty]");
                    }

                    if (unit.data.magicOptionsForUnit.Count > 0)
                    {
                        tag = $@"{unit.data.title}\Magic\";
                        options.AddRange(unit.data.magicOptionsForUnit.Select(magic =>
                            tag + GameUIManager.GetCleanedText(magic.ToString())));
                    }
                    else
                    {
                        options.Add($"{unit.data.title}\\Magic [unavailable]");
                    }
                }

                yield return GameUIManager.FullMenu(options.ToArray(), true);

                yield return new WaitUntil(() => MyInput.Select == 0);
            }
        }
    }
}