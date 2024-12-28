using System;

namespace Managers.CutScene
{
    public partial class CutScene
    {
        public class Window : IDisposable
        {
            private static CutScene CutSceneInstance => _i;
            private readonly bool hardMode;

            public Window()
            {
                if (!Enabled)
                {
                    CutSceneInstance.onEnable?.Invoke();
                }

                CutSceneInstance.activeWindows += 1;
            }

            public Window(bool hardMode) : this()
            {
                if (!hardMode) return;
                CutSceneInstance.activeHardWindows += 1;
                this.hardMode = true;
            }

            public void Dispose()
            {
                if (CutSceneInstance.activeWindows > 0)
                    CutSceneInstance.activeWindows -= 1;

                if (hardMode)
                    CutSceneInstance.activeHardWindows -= 1;
            }
        }
    }
}