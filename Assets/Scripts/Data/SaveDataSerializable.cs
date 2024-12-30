using System.Collections.Generic;
using System.Linq;
using Battle;
using Controllers;
using Managers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Data
{
    [System.Serializable]
    public class SaveDataSerializable
    {
        public int id;
        public BattleUnit.BattleUnitData battleUnitData;
        public BattleUnit.BattleUnitData[] comradeBattleUnitData;
        public byte[] comradeBattleUnitSpriteIdx;
        public float[] position;
        public LevelScene levelScene;
        public string[] checkpoints;
        public int[] npcActionTreeBranchProtectors;

        public SaveDataSerializable()
        {
            // Save player position
            Vector2 position = PlayerController.playerController.transform.position;
            this.position = new float[2] { position.x, position.y };
            levelScene = (LevelScene)System.Enum.Parse(typeof(LevelScene), SceneManager.GetActiveScene().name);

            // Save GameManager
            id = GameManager.Id;

            // Save player data
            battleUnitData = PlayerManager.GetBattleUnitData();

            // Save checkpoints
            checkpoints = (from checkpoint in CheckpointSystem.checkpoints where checkpoint.isReached select checkpoint.checkpoint).ToArray();

            // Save NPCs talked to
            npcActionTreeBranchProtectors = GameManager.PostInteractionProtectionIDs.ToArray();

            // Save player comrades
            comradeBattleUnitData = PlayerManager.ComradeBattleUnits.Select(x => x.data).ToArray();
            comradeBattleUnitSpriteIdx = PlayerManager.ComradeBattleUnits.Select(x => NpcSpriteIDMap.GetID(x.sprite)).ToArray();
        }

        public static void UnpackSaveData(SaveDataSerializable data)
        {
            GameManager.SetId(data.id);
            GameManager.SetPostInteractionProtectionIDs(new HashSet<int>(data.npcActionTreeBranchProtectors));
            PlayerManager.SetBattleUnitData(data.battleUnitData);

            foreach (string checkpoint in data.checkpoints)
            {
                CheckpointSystem.SetCheckpoint(checkpoint);
            }

            for (int i=0; i < data.comradeBattleUnitData.Length; i++)
            {
                byte spriteID = data.comradeBattleUnitSpriteIdx[i];
                BattleUnit.BattleUnitData battleUnitData = data.comradeBattleUnitData[i];
                PlayerManager.AddBattleUnit(battleUnitData, NpcSpriteIDMap.GetSprite(spriteID));
            }
        }

    }
}
