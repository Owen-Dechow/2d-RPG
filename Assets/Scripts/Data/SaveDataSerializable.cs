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
        public float[] position;
        public LevelScene levelScene;
        public string[] checkpoints;
        public int[] postInteractionProtectionIDs;

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
            checkpoints = (from checkpoint in CheckpointSystem.checkpoints
                where checkpoint.isReached
                select checkpoint.checkpoint).ToArray();

            // Save NPCs talked to
            postInteractionProtectionIDs = GameManager.PostInteractionProtectionIDs.ToArray();

            // Save player comrades
            comradeBattleUnitData = PlayerManager.ComradeBattleUnits.Select(x => x.GetSyncedData()).ToArray();
        }

        public static void UnpackSaveData(SaveDataSerializable data)
        {
            GameManager.SetId(data.id);
            GameManager.SetPostInteractionProtectionIDs(new HashSet<int>(data.postInteractionProtectionIDs));
            PlayerManager.SetBattleUnitData(data.battleUnitData);
            
            foreach (string checkpoint in data.checkpoints)
            {
                CheckpointSystem.SetCheckpoint(checkpoint);
            }

            foreach (var battleUnitData in data.comradeBattleUnitData)
            {
                PlayerManager.AddBattleUnit(battleUnitData);
            }
        }
    }
}