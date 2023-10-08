using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SaveDataSerializable
{
    public int id;
    public BattleUnit.BattleUnitData battleUnitData;
    public BattleUnit.BattleUnitData[] comradeBattleUnitData;
    public byte[] comradeBattleUnitSpriteIDX;
    public float[] position;
    public LevelScene levelScene;
    public string[] checkpoints;
    public int[] NPCActionTreeBranchProtectors;

    public SaveDataSerializable()
    {
        // Save player position
        Vector2 position = PlayerController.playerController.transform.position;
        this.position = new float[2] { position.x, position.y };
        levelScene = (LevelScene)System.Enum.Parse(typeof(LevelScene), SceneManager.GetActiveScene().name);

        // Save GameManager
        id = GameManager.id;

        // Save player data
        battleUnitData = Player.GetBattleUnitData();

        // Save checkpoints
        List<string> checkpointsReached = new();
        foreach (CheckpointSystem.Checkpoint checkpoint in CheckpointSystem.Checkpoints)
        {
            if (checkpoint.isReached) checkpointsReached.Add(checkpoint.checkpoint);
        }
        checkpoints = checkpointsReached.ToArray();

        // Save NPCs talked to
        NPCActionTreeBranchProtectors = GameManager.NPCActionTreeBranchProtectors.ToArray();

        // Save player comrades
        comradeBattleUnitData = Player.ComradeBattleUnits.Select(x => x.data).ToArray();
        comradeBattleUnitSpriteIDX = Player.ComradeBattleUnits.Select(x => NPCSpriteIDMap.GetID(x.sprite)).ToArray();
    }

    public static void UnpackSaveData(SaveDataSerializable data)
    {
        GameManager.id = data.id;
        GameManager.NPCActionTreeBranchProtectors = new(data.NPCActionTreeBranchProtectors);
        Player.SetBattleUnitData(data.battleUnitData);

        foreach (string checkpoint in data.checkpoints)
        {
            CheckpointSystem.SetCheckpoint(checkpoint);
        }

        for (int i=0; i < data.comradeBattleUnitData.Length; i++)
        {
            byte spriteID = data.comradeBattleUnitSpriteIDX[i];
            BattleUnit.BattleUnitData battleUnitData = data.comradeBattleUnitData[i];
            Player.AddBattleUnit(battleUnitData, NPCSpriteIDMap.GetSprite(spriteID));
        }
    }

}
