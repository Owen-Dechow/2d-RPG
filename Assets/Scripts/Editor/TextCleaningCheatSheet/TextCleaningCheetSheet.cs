using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class TextCleaningCheetSheet : EditorWindow
{
    [MenuItem("Custom/TextCleaningCheatSheet")]
    public static void ShowExample()
    {
        TextCleaningCheetSheet wnd = GetWindow<TextCleaningCheetSheet>();
        wnd.titleContent = new GUIContent("TextCleaningCheatSheet");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/TextCleaningCheatSheet/TextCleaningCheetSheet.uxml");
        VisualElement labelFromUXML = visualTree.Instantiate();
        root.Add(labelFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/TextCleaningCheatSheet/TextCleaningCheetSheet.uss");
        root.styleSheets.Add(styleSheet);
    }
}