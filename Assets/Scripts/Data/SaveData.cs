using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SaveData
{
    public int id;
    public BattleUnit.BattleUnitData battleUnitData;
    public float[] position;
    public LevelScene levelScene;
    public string[] checkpoints;
    public int[] NPCActionTreeBranchProtectors;

    public SaveData()
    {
        Player player = GameManager.player;
        BattleUnit playerBattleUnit = player.playerBattleUnit;
        Vector2 position = player.playerObject.transform.position;

        id = GameManager.id;
        battleUnitData = playerBattleUnit.data;

        this.position = new float[2] { position.x, position.y };

        List<string> checkpointsReached = new();
        foreach (CheckpointSystem.Checkpoint checkpoint in GameManager.checkpoints.checkpoints)
        {
            if (checkpoint.isReached) checkpointsReached.Add(checkpoint.checkpoint);
        }
        checkpoints = checkpointsReached.ToArray();

        levelScene = (LevelScene)System.Enum.Parse(typeof(LevelScene), SceneManager.GetActiveScene().name);

        NPCActionTreeBranchProtectors = GameManager.NPCActionTreeBranchProtectors.ToArray();
    }

    public static void UnpackSaveData(SaveData data)
    {
        Player player = GameManager.player;
        BattleUnit playerBattleUnit = player.playerBattleUnit;

        GameManager.id = data.id;
        GameManager.NPCActionTreeBranchProtectors = new(data.NPCActionTreeBranchProtectors);
        playerBattleUnit.data = data.battleUnitData;

        foreach (string checkpoint in data.checkpoints)
        {
            GameManager.checkpoints.SetCheckpoint(checkpoint);
        }
    }

}

public static class SaveSystem
{
    public static readonly string path = Application.persistentDataPath + "/dechow-rpg.savedata";

    public static void SaveData(SaveData data)
    {

        SaveData[] dataSlots = GetDataSlots();
        SaveData oldDataSlot = null;
        int oldDataSlotIDX = 0;

        if (File.Exists(path))
        {
            // Check if already save data 
            foreach (SaveData dataSlot in dataSlots)
            {
                if (dataSlot.id == data.id)
                {
                    oldDataSlot = data;
                    break;
                }
                oldDataSlotIDX++;
            }
        }
        else
        {
            dataSlots = new SaveData[0];
        }

        // Replace or create save data
        SaveData[] newDataSlots;
        if (oldDataSlot == null)
        {
            newDataSlots = new SaveData[dataSlots.Length + 1];
            dataSlots.CopyTo(newDataSlots, 0);
            newDataSlots[^1] = data;
        }
        else
        {
            newDataSlots = dataSlots;
            newDataSlots[oldDataSlotIDX] = data;
        }

        SetDataSlots(newDataSlots);
    }

    public static SaveData LoadData(int id)
    {
        SaveData[] dataSlots = GetDataSlots();

        // Find proper save profile
        foreach (SaveData data in dataSlots)
        {
            if (data.id == id) return data;
        }
        Debug.LogError("Savedata not found at path: " + path);
        return null;
    }

    public static int GetNewId()
    {
        int id = Random.Range(0, int.MaxValue);

        SaveData[] dataSlots = GetDataSlots();

        // Find proper save profile
        foreach (SaveData data in dataSlots)
        {
            if (data.id == id) return GetNewId();
        }
        return id;
    }

    public static Dictionary<int, string> GetSaveProfiles()
    {
        Dictionary<int, string> profiles = new();
        if (File.Exists(path))
        {
            // Get save data
            SaveData[] dataSlots = GetDataSlots();

            foreach (SaveData data in dataSlots)
            {
                profiles[data.id] = data.battleUnitData.title;
            }
        }
        return profiles;
    }

    public static void RemoveSaveProfile(int key)
    {
        SaveData[] dataSlots = GetDataSlots();
        SaveData[] newDataSlots;

        int idxOn = 0;
        newDataSlots = new SaveData[dataSlots.Length - 1];
        foreach (SaveData dataSlot in dataSlots)
        {
            if (dataSlot.id == key) continue;
            newDataSlots[idxOn] = dataSlot;
            idxOn += 1;
        }

        SetDataSlots(newDataSlots);
    }

    private static SaveData[] GetDataSlots()
    {
        if (!File.Exists(path))
        {
            Debug.Log("No file found at path :" + path);
            return new SaveData[0];
        }

        BinaryFormatter formatter = new();
        FileStream stream = new(path, FileMode.Open);

        SaveData[] data = formatter.Deserialize(stream) as SaveData[];
        stream.Close();
        return data;
    }
    private static void SetDataSlots(SaveData[] dataSlots)
    {
        try
        {
            BinaryFormatter formatter = new();
            FileStream stream = new(path, FileMode.Create);
            formatter.Serialize(stream, dataSlots);
            stream.Close();
        }
        catch
        {
            Debug.LogError("File saving error at path :" + path);
            return;
        }
    }
}
