// Ignore Spelling: Npc

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;


public class Npc : MonoBehaviour
{
    public enum MovementType
    {
        FaceDown,
        FaceUp,
        FaceLeft,
        FaceRight,
        FacePlayer,
        XOnly,
        YOnly,
        XAndY,
    }

    [SerializeField] MovementType movementType;
    public BehaviorTree behaviorTree;

    [SerializeField] string NPCEnabledCheckpointWindowOpen;
    [SerializeField] string NPCEnabledCheckpointWindowClose;

    private bool inPlayerInteractionZone = false;
    private bool runningInteraction = false;
    
    private SpriteRenderer spriteRenderer;
    private float spriteRendererAlfa;

    private void Start()
    {
        if (!CheckpointSystem.GetWindow(NPCEnabledCheckpointWindowOpen, NPCEnabledCheckpointWindowClose))
            Destroy(gameObject);

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        InteractWithPlayer();
    }

    IEnumerator DestroySlow()
    {
        int iterations = 6;
        for (float i = 0; i < iterations; i++)
        {
            spriteRenderer.color = new Color(1, 1, 1, Mathf.Clamp(1 - (i / iterations), 0, 1));
            yield return new WaitForSecondsRealtime(0.1f);
            spriteRenderer.color = Color.clear;
            yield return new WaitForSecondsRealtime(0.05f);
        }

        Destroy(gameObject);
    }

    void InteractWithPlayer()
    {
        if (MyInput.SelectDown != 1) return;
        if (runningInteraction) return;
        if (!inPlayerInteractionZone) return;
        if (Time.timeScale == 0) return;
        StartCoroutine(RunInteraction());
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


    IEnumerator RunInteraction()
    {
        Time.timeScale = 0;
        runningInteraction = true;
        yield return new WaitForEndOfFrame();

        yield return behaviorTree.Run();

        yield return new WaitWhile(() => MyInput.Select == 1);

        if (!CheckpointSystem.GetWindow(NPCEnabledCheckpointWindowOpen, NPCEnabledCheckpointWindowClose))
        {
            yield return DestroySlow();
        }

        Time.timeScale = 1;
        runningInteraction = false;
    }
}