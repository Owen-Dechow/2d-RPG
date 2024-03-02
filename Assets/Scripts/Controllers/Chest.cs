using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
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
        open = GameManager.PostInteractionProtectionIDs.Contains(uniqueId);
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
        if (Time.timeScale == 0) return;
        if (open) return;

        StartCoroutine(RunInteraction());
    }

    IEnumerator RunInteraction()
    {
        Time.timeScale = 0;
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
                yield return GameUI.TypeOut($"{Player.Name} found {itemOption}, but didn't have space in inventory.");
                spriteRenderer.sprite = closedSprite;
            }
        }

        Time.timeScale = 1;
    }

    void SetOpen()
    {
        GameManager.PostInteractionProtectionIDs.Add(uniqueId);
        open = true;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.CompareTag("Player")) return;
        inPlayerInteractionZone = true;
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (!collider.CompareTag("Player")) return;
        inPlayerInteractionZone = false;
    }
}
