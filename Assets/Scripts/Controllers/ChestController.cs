using System.Collections;
using Managers;
using UnityEngine;

namespace Controllers
{
    public class ChestController : MonoBehaviour
    {
        public enum ContentType
        {
            Item,
            Gold,
            Null,
        }

        public Sprite openSprite;
        public Sprite closedSprite;
        public int uniqueId;

        public ContentType contentType;
        public GameItems.Options itemOption;
        public int goldOption;

        SpriteRenderer spriteRenderer;
        bool open;
        bool inPlayerInteractionZone;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            inPlayerInteractionZone = false;
            open = GameManager.postInteractionProtectionIDs.Contains(uniqueId);
            if (open)
                spriteRenderer.sprite = openSprite;
            else
                spriteRenderer.sprite = closedSprite;
        }


        private void Update()
        {
            InteractWithPlayer();
        }

        void InteractWithPlayer()
        {
            if (MyInput.SelectDown != 1) return;
            if (!inPlayerInteractionZone) return;
            if (CutScene.Enabled) return;
            if (open) return;

            StartCoroutine(RunInteraction());
        }

        IEnumerator RunInteraction()
        {
            using (new CutScene.Window())
            {
                yield return new WaitForEndOfFrame();
                spriteRenderer.sprite = openSprite;

                if (contentType == ContentType.Gold)
                {
                    Player.Gold += goldOption;
                    yield return GameUI.TypeOut($"{Player.Name} found {goldOption} gold.");
                    SetOpen();
                }
                else if (contentType == ContentType.Item)
                {
                    if (Player.HasRoomInInventory())
                    {
                        Player.AddItemToInventory(itemOption);
                        yield return GameUI.TypeOut($"{Player.Name} found {itemOption}.");
                        SetOpen();
                    }
                    else
                    {
                        yield return GameUI.TypeOut(
                            $"{Player.Name} found {itemOption}, but didn't have space in inventory.");
                        spriteRenderer.sprite = closedSprite;
                    }
                }
            }
        }

        void SetOpen()
        {
            GameManager.postInteractionProtectionIDs.Add(uniqueId);
            open = true;
        }

        void OnTriggerEnter2D(Collider2D collider2d)
        {
            if (!collider2d.CompareTag("Player")) return;
            inPlayerInteractionZone = true;
        }

        void OnTriggerExit2D(Collider2D collider2d)
        {
            if (!collider2d.CompareTag("Player")) return;
            inPlayerInteractionZone = false;
        }
    }
}