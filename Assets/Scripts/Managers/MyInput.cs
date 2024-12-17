using System.Collections;
using UnityEngine;

namespace Managers
{
    public static class MyInput
    {
        public enum Action
        {
            Up,
            Down,
            Left,
            Right,
            Select,
            Cancel,
            None,
        }

        public static int Select => GetSelect();
        public static int SelectDown => GetSelectDown();
        public static float MoveHorizontal => GetMove_Horizontal();
        public static float MoveVertical => GetMove_Vertical();
        public static bool OpenMenu => GetOpenMenu();

        public static Action MenuNavigation { get; private set; } = Action.None;

        static float GetMove_Horizontal()
        {
            return Input.GetAxisRaw("Horizontal");
        }
        static float GetMove_Vertical()
        {
            return Input.GetAxisRaw("Vertical");
        }

        static int GetSelect()
        {
            return (int)(Input.GetAxisRaw("Submit") - Input.GetAxisRaw("Cancel"));
        }

        static int GetSelectDown()
        {
            if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Cancel"))
                return GetSelect();
            else return 0;
        }

        static bool GetOpenMenu()
        {
            return Input.GetKeyDown(KeyCode.Z);
        }

        public static IEnumerator WaitForMenuNavigation()
        {
            yield return new WaitUntil(() =>
                Input.GetButtonDown("Horizontal")
                || Input.GetButtonDown("Vertical")
                || Input.GetButtonDown("Submit")
                || Input.GetButtonDown("Cancel")
            );

            if (MoveHorizontal == 1) MenuNavigation = Action.Right;
            else if (MoveHorizontal == -1) MenuNavigation = Action.Left;
            else if (MoveVertical == 1) MenuNavigation = Action.Up;
            else if (MoveVertical == -1) MenuNavigation = Action.Down;
            else if (Select == 1) MenuNavigation = Action.Select;
            else if (Select == -1) MenuNavigation = Action.Cancel;
            else MenuNavigation = Action.None;
        }
        public static IEnumerator WaitForClickSelect()
        {
            yield return new WaitUntil(() => GetSelectDown() != 0);
            yield return new WaitWhile(() => GetSelect() != 0);
        }
    }
}
