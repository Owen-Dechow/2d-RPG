using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Npc : MonoBehaviour
{
    //public enum ActionType
    //{
    //    GiveItem,
    //    TakeItem,
    //}

    public enum MovementType
    {
        None,
        FacePlayer,
        XOnly,
        YOnly,
        XAndY,
    }
    [SerializeField] MovementType movementType;
    public BehaviourTree behaviourTree;

    private bool inPlayerInteractionZone = false;
    private bool runningInteraction = false;

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

    private void Update()
    {
        InteractWithPlayer();
    }

    void InteractWithPlayer()
    {
        if (MyInput.GetSelectDown() != 1) return;
        if (runningInteraction) return;
        if (!inPlayerInteractionZone) return;
        if (Time.timeScale == 0) return;
        StartCoroutine(RunInteraction());
    }

    IEnumerator RunInteraction()
    {
        Time.timeScale = 0;
        runningInteraction = true;
        yield return new WaitForEndOfFrame();

        yield return behaviourTree.Run();

        yield return new WaitWhile(() => MyInput.GetSelect() == 1);
        Time.timeScale = 1;
        GameManager.CloseUI();
        runningInteraction = false;
    }
}