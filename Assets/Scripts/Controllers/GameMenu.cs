using System.Collections;
using UnityEngine;

namespace Controllers
{
    public class GameMenu : MonoBehaviour
    {
        private static GameMenu _i;

        void Start()
        {
            _i = this;
        }

        public static IEnumerator OpenMenuStatic() => _i.OpenMenu();

        IEnumerator OpenMenu()
        {
            yield return null;
            //print(GameManager.i);
        }
    }
}
