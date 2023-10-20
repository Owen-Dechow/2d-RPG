using System.Collections;
using UnityEngine;

public class Node_ChangeScene : ActionNode
{
    [SerializeField] LevelScene scene;
    [SerializeField] Vector2 playerSpanPoint;
    

    protected override IEnumerator Execute(Npc npc)
    {        
        npc.transform.parent = null;
        DontDestroyOnLoad(npc);
        
        yield return GameManager.LoadLevelAnimated(scene, playerSpanPoint, AnimPlus.Direction.down);
        Destroy(npc.gameObject);
    }
}
