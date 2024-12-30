using System.IO;
using Managers;
using UnityEditor;

public class DeleteSaveData
{
    private const string DeleteSaveDataStr = "Custom/Delete Save Data";

    [MenuItem(DeleteSaveDataStr, false, 200)]
    private static void DeleteSaveDataMenu()
    {
        if (EditorUtility.DisplayDialog("Delete Save Data", $"Confirm deletion of save data at {SaveSystem.Path}", "OK", "Cancel"))
        {
           File.Delete(SaveSystem.Path); 
        }
    }
}