using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using Managers.CutScene;
using UnityEngine;

namespace Controllers
{
    public class SavePoint : MonoBehaviour
    {
        private float active = 0;
        private SpriteRenderer sr;
        [SerializeField] private Gradient gradient;
        [SerializeField] private float colorSpeed;

        private void Start()
        {
            sr = GetComponent<SpriteRenderer>();
        }
        
        private void Update()
        {
            active += Time.deltaTime;
            sr.color = gradient.Evaluate(Mathf.Sin(Time.time * colorSpeed) / 2 + .5f);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!collision.collider.CompareTag("Player")) return;
            if (active < 0.5f) return;
            StartCoroutine(SaveGame());
        }

        IEnumerator SaveGame()
        {
            using (new CutScene.Window())
            {
                yield return GameUI.GetYesNo("Save Game?");
                if (GameUI.Answer == "Yes") SaveSystem.SaveGame();
            }
        }
    }
}