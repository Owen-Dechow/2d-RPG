using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SaveData
{
    public int _id;
    public string _name;
    public int _life;
    public int _maxLife;
    public int _magic;
    public int _maxMagic;
    public BattleUnit.UnitRefexive _reflexive;
    public float[] _position;
    public Magic.Options[] _magicOptions;
    public Items.Options[] _items;
    public Badges.Equip _badgesEquipped;
    public Badges.Inventory _badgeInventory;
    public LevelScene _levelScene;
    public string[] _checkpoints;
    public int[] _NPCActionTreeBranchProtectors;

    public SaveData()
    {
        Player player = GameManager.player;
        BattleUnit playerBattleUnit = player.playerBattleUnit;
        Vector2 position = player.playerObject.transform.position;

        _id = GameManager.id;
        _name = playerBattleUnit.title;
        _life = playerBattleUnit.life;
        _maxLife = playerBattleUnit.maxLife;
        _magic = playerBattleUnit.magic;
        _maxMagic = playerBattleUnit.maxMagic;
        _magicOptions = playerBattleUnit.magicOptionsForUnit.ToArray();
        _items = playerBattleUnit.items.ToArray();
        _badgesEquipped = playerBattleUnit.badges;
        _reflexive = playerBattleUnit.refexive;

        _position = new float[2] { position.x, position.y };
        _badgeInventory = player.badgeInventory;

        List<string> checkpointsReached = new();
        foreach (CheckpointSystem.Checkpoint checkpoint in GameManager.checkpoints.checkpoints)
        {
            if (checkpoint.isReached) checkpointsReached.Add(checkpoint.checkpoint);
        }
        _checkpoints = checkpointsReached.ToArray();

        _levelScene = (LevelScene)System.Enum.Parse(typeof(LevelScene), SceneManager.GetActiveScene().name);

        _NPCActionTreeBranchProtectors = GameManager.NPCActionTreeBranchProtectors.ToArray();
    }

    public static void UnpackSaveData(SaveData data)
    {
        Player player = GameManager.player;
        BattleUnit playerBattleUnit = player.playerBattleUnit;

        GameManager.id = data._id;
        GameManager.NPCActionTreeBranchProtectors = new(data._NPCActionTreeBranchProtectors);
        playerBattleUnit.title = data._name;
        playerBattleUnit.life = data._life;
        playerBattleUnit.maxLife = data._maxLife;
        playerBattleUnit.magic = data._magic;
        playerBattleUnit.maxMagic = data._maxMagic;
        playerBattleUnit.magicOptionsForUnit = new List<Magic.Options>(data._magicOptions);
        playerBattleUnit.items = new List<Items.Options>(data._items);
        playerBattleUnit.badges = data._badgesEquipped;
        playerBattleUnit.refexive = data._reflexive;

        player.badgeInventory = data._badgeInventory;

        foreach (string checkpoint in data._checkpoints)
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
                if (dataSlot._id == data._id)
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
            if (data._id == id) return data;
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
            if (data._id == id) return GetNewId();
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
                profiles[data._id] = data._name;
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
            if (dataSlot._id == key) continue;
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
            Debug.Log("File saving error at path :" + path);
            return;
        }
    }
}
