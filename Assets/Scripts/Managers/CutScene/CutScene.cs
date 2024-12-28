using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Managers.CutScene
{
    public partial class CutScene : MonoBehaviour
    {
        private static CutScene _i;

        [SerializeField] private byte activeWindows;
        [SerializeField] private byte activeHardWindows;

        public static bool Enabled => _i.activeWindows > 0 || _i.activeHardWindows > 0;

        private UnityAction onEnable;

        public static UnityAction OnEnable
        {
            get => _i.onEnable;
            set => _i.onEnable = value;
        }

        private void Awake()
        {
            _i = this;
        }

        public static void DisableAllSoftCutscenes()
        {
            _i.activeWindows = 0;
        }
    }
}