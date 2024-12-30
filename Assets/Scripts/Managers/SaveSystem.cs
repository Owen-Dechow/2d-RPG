using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public static class SaveSystem 
    {
        public static string Path => Application.persistentDataPath + "/game.savedata";

        public static void SaveGame()
        {
            SaveDataSerializable saveData = new();

            SaveDataSerializable[] dataSlots = GetDataSlots();
            SaveDataSerializable oldDataSlot = null;
            int oldDataSlotIdx = 0;

            if (File.Exists(Path))
            {
                // Check if already save data 
                foreach (SaveDataSerializable dataSlot in dataSlots)
                {
                    if (dataSlot.id == saveData.id)
                    {
                        oldDataSlot = saveData;
                        break;
                    }
                    oldDataSlotIdx++;
                }
            }
            else
            {
                dataSlots = Array.Empty<SaveDataSerializable>();
            }

            // Replace or create save data
            SaveDataSerializable[] newDataSlots;
            if (oldDataSlot == null)
            {
                newDataSlots = new SaveDataSerializable[dataSlots.Length + 1];
                dataSlots.CopyTo(newDataSlots, 0);
                newDataSlots[^1] = saveData;
            }
            else
            {
                newDataSlots = dataSlots;
                newDataSlots[oldDataSlotIdx] = saveData;
            }

            SetDataSlots(newDataSlots);
            Debug.Log("Game Saved: [path] " + SaveSystem.Path);
        }

        public static SaveDataSerializable LoadData(int id)
        {
            SaveDataSerializable[] dataSlots = GetDataSlots();

            // Find proper save profile
            foreach (SaveDataSerializable data in dataSlots)
            {
                if (data.id == id) return data;
            }
            Debug.LogError("File not found at path: " + Path);
            return null;
        }

        public static int GetNewId()
        {
            int id = Random.Range(0, int.MaxValue);

            SaveDataSerializable[] dataSlots = GetDataSlots();

            // Find proper save profile
            foreach (SaveDataSerializable data in dataSlots)
            {
                if (data.id == id) return GetNewId();
            }
            return id;
        }

        public static Dictionary<int, string> GetSaveProfiles()
        {
            Dictionary<int, string> profiles = new();
            if (File.Exists(Path))
            {
                // Get save data
                SaveDataSerializable[] dataSlots = GetDataSlots();

                foreach (SaveDataSerializable data in dataSlots)
                {
                    profiles[data.id] = data.battleUnitData.title;
                }
            }
            return profiles;
        }

        public static void RemoveSaveProfile(int key)
        {
            SaveDataSerializable[] dataSlots = GetDataSlots();
            SaveDataSerializable[] newDataSlots;

            int idxOn = 0;
            newDataSlots = new SaveDataSerializable[dataSlots.Length - 1];
            foreach (SaveDataSerializable dataSlot in dataSlots)
            {
                if (dataSlot.id == key) continue;
                newDataSlots[idxOn] = dataSlot;
                idxOn += 1;
            }

            SetDataSlots(newDataSlots);
        }

        private static SaveDataSerializable[] GetDataSlots()
        {
            if (!File.Exists(Path))
            {
                Debug.Log("No file found at path :" + Path);
                return new SaveDataSerializable[0];
            }

            BinaryFormatter formatter = new();
            FileStream stream = new(Path, FileMode.Open);

            SaveDataSerializable[] data = formatter.Deserialize(stream) as SaveDataSerializable[];
            stream.Close();
            return data;
        }
        private static void SetDataSlots(SaveDataSerializable[] dataSlots)
        {
            try
            {
                BinaryFormatter formatter = new();
                FileStream stream = new(Path, FileMode.Create);
                formatter.Serialize(stream, dataSlots);
                stream.Close();
            }
            catch
            {
                Debug.LogError("File saving error at path :" + Path);
                return;
            }
        }
    }
}
