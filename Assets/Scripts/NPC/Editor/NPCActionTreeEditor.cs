using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Callbacks;


public class NPCActionTreeEditor : EditorWindow
{
    TreeGraph treeView;

    [MenuItem("Custom/NPC Action Tree Editor")]
    public static void ShowExample()
    {
        NPCActionTreeEditor wnd = GetWindow<NPCActionTreeEditor>();
        wnd.titleContent = new GUIContent("NPCActionTreeEditor");
        wnd.Show();
    }

    [OnOpenAsset]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter")]
    public static bool OnOpenAsset(int instanceId, int line)
    {

        if (Selection.activeObject is BehaviourTree)
        {
            ShowExample();
            return true;
        }
        return false;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/NPC/Editor/NPCActionTreeEditor.uxml");
        visualTree.CloneTree(root);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/NPC/Editor/NPCActionTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        treeView = root.Q<TreeGraph>();

        OnSelectionChange();
    }

    private void OnSelectionChange()
    {
        BehaviourTree tree = Selection.activeObject as BehaviourTree;
        Label title = rootVisualElement.Q<Label>("tree-name");
        Label info = rootVisualElement.Q<Label>("tree-info");

        if (tree != null && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
        {
            treeView.PopulateGraph(tree);
            title.text = tree.name;
            title.RemoveFromClassList("red-text");
            info.RemoveFromClassList("hide");
            info.text = $"{tree}\n" +
                $"Number of nodes: {tree.nodes.Count}\n" +
                $"Instance Id: {tree.GetInstanceID()}\n" +
                $"RootnNode: {tree.rootNode}";
        }
    }
}