using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Managers
{
    public class CutScene : MonoBehaviour
    {
        private static CutScene _i;

        [SerializeField] private new bool enabled;
        public static bool Enabled => _i.enabled;

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

        public class Window : IDisposable
        {
            private bool initial;

            public Window()
            {
                initial = CutScene.Enabled;

                if (!_i.enabled)
                {
                    _i.onEnable?.Invoke();
                    _i.enabled = true;
                }
            }

            public void Dispose()
            {
                _i.enabled = initial;
            }
        }
    }
}