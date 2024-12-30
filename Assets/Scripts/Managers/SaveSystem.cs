using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Managers
{
    public static class SaveSystem
    {
        private static string PathRoot => Application.persistentDataPath + "/game.";
        public static string Path => PathRoot + (GameManager.SaveAsJson ? "json" : "savedata");

        public static void SaveGame()
        {
            SaveDataSerializable saveData = new();

            List<SaveDataSerializable> dataSlots = GetDataSlots();
            SaveDataSerializable oldDataSlot = dataSlots.FirstOrDefault(x => x.id == saveData.id);

            if (oldDataSlot == null)
            {
                dataSlots.Add(saveData);
            }
            else
            {
                dataSlots.Remove(oldDataSlot);
                dataSlots.Add(saveData);
            }

            SetDataSlots(dataSlots);
            Debug.Log("Game Saved: [path] " + SaveSystem.Path);
        }

        public static SaveDataSerializable LoadData(int id)
        {
            List<SaveDataSerializable> dataSlots = GetDataSlots();

            // Find proper save profile
            foreach (var data in dataSlots.Where(data => data.id == id))
            {
                return data;
            }

            Debug.LogError("File not found at path: " + Path);
            return null;
        }

        public static int GetNewId()
        {
            return Random.Range(int.MinValue, int.MaxValue);
        }

        public static Dictionary<int, string> GetSaveProfiles()
        {
            Dictionary<int, string> profiles = new();
            List<SaveDataSerializable> dataSlots = GetDataSlots();

            foreach (SaveDataSerializable data in dataSlots)
            {
                profiles[data.id] = data.battleUnitData.title;
            }

            return profiles;
        }

        public static void RemoveSaveProfile(int key)
        {
            List<SaveDataSerializable> dataSlots = GetDataSlots();
            dataSlots.RemoveAll(x => x.id == key);
            SetDataSlots(dataSlots);
        }

        private static List<SaveDataSerializable> GetDataSlots()
        {
            if (!File.Exists(Path))
            {
                Debug.Log("No file found at path :" + Path);
                return new List<SaveDataSerializable>();
            }

            using FileStream stream = new(Path, FileMode.Open);

            if (GameManager.SaveAsJson)
            {
                DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(List<SaveDataSerializable>));
                return (List<SaveDataSerializable>)formatter.ReadObject(stream);
            }
            else
            {
                BinaryFormatter formatter = new();
                return (List<SaveDataSerializable>)formatter.Deserialize(stream);
            }
        }

        private static void SetDataSlots(List<SaveDataSerializable> dataSlots)
        {
            try
            {
                using FileStream stream = new(Path, FileMode.Create);
                if (GameManager.SaveAsJson)
                {
                    DataContractJsonSerializer formatter =
                        new DataContractJsonSerializer(typeof(List<SaveDataSerializable>));
                    formatter.WriteObject(stream, dataSlots);
                }
                else
                {
                    BinaryFormatter formatter = new();
                    formatter.Serialize(stream, dataSlots);
                }
            }
            catch
            {
                Debug.LogError("File saving error at path :" + Path);
            }
        }
    }
}