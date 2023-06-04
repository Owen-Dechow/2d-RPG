using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenu : MonoBehaviour
{
    static GameMenu i;

    void Start()
    {
        i = this;
    }

    public static IEnumerator OpenMenuStatic() => i.OpenMenu();

    IEnumerator OpenMenu()
    {
        yield return null;
        //print(GameManager.i);
    }
}
