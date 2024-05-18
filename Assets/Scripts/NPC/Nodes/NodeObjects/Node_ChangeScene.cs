using System.Collections;
using UnityEngine;

public class Node_ChangeScene : ActionNode
{
    [SerializeField] LevelScene scene;
    [SerializeField] Vector2 playerSpanPoint;
    [SerializeField] bool saveAfterChange = true;

    public override string MenuLocation => "Animation/Change Scene";

    protected override IEnumerator Execute(Npc npc, BehaviorTree.TreeData treeData)
    {        
        npc.transform.parent = null;
        npc.GetComponent<Renderer>().enabled = false;
        DontDestroyOnLoad(npc);
        
        yield return GameManager.LoadLevelAnimated(scene, playerSpanPoint, AnimPlus.Direction.down);
        
        if (saveAfterChange)
            SaveSystem.SaveGame();

        Destroy(npc.gameObject);
    }
}
