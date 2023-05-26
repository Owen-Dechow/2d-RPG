using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyInput
{
    public static float GetMoveHorizontal()
    {
        return Input.GetAxisRaw("Horizontal");
    }
    public static float GetMoveVertical()
    {
        return Input.GetAxisRaw("Vertical");
    }

    public static int GetSelect()
    {
        return (int)(Input.GetAxisRaw("Submit") - Input.GetAxisRaw("Cancel"));
    }

    public static int GetSelectDown()
    {
        if (!Input.anyKeyDown)
        {
           return GetSelect();
        } else return 0;
    }

    public static IEnumerator WaitForClick()
    {
        yield return new WaitUntil(() => GetSelectDown() == 1);
        yield return new WaitWhile(() => GetSelect() == 1);
    }
}
