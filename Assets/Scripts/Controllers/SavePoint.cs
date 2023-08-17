using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    float active = 0;

    private void Update()
    {
        active += Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;
        if (active < 0.5f) return;
        StartCoroutine(SaveGame());
    }

    IEnumerator SaveGame()
    {
        Time.timeScale = 0;
        yield return GameUI.GetYesNo("Save Game?");
        if (GameManager.Answer == "Yes") GameManager.SaveGame();
        Time.timeScale = 1;
    }
}
