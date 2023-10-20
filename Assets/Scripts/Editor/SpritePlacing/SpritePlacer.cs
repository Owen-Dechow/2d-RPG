using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class SpritePlacer : EditorWindow
{
    enum GameObjectType
    {
        Enemy,
        NPC,
        Other,
    }

    PlacerSettings placerSettings;
    bool m_IsDragPerformed = false;
    bool m_IsDragging = false;
    GameObjectType m_GameObjectType;

    [MenuItem("Custom/SpritePlacer")]
    public static void ShowWindow()
    {
        SpritePlacer wnd = GetWindow<SpritePlacer>();
        wnd.titleContent = new GUIContent("SpritePlacer");
    }

    public void OnEnable()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/SpritePlacing/SpritePlacer.uxml");
        VisualElement uxml = visualTree.Instantiate();
        root.Add(uxml);

        placerSettings = AssetDatabase.LoadAssetAtPath<PlacerSettings>("Assets/Scripts/Editor/SpritePlacing/PlacerSettings.asset");

        foreach (GameObject prefab in placerSettings.NPCs)
        {
            CreateDragAndDrop(prefab, placerSettings, "NPCs", GameObjectType.NPC);
        }
        foreach (GameObject prefab in placerSettings.enemies)
        {
            CreateDragAndDrop(prefab, placerSettings, "enemies", GameObjectType.Enemy);
        }
        foreach (GameObject prefab in placerSettings.other)
        {
            CreateDragAndDrop(prefab, placerSettings, "other", GameObjectType.Other);
        }

        SceneView.duringSceneGui += sv => OnDragEnd();
        EditorApplication.hierarchyWindowItemOnGUI += (id, rect) => OnDragEnd();
    }

    void CreateDragAndDrop(GameObject prefab, PlacerSettings settings, string container, GameObjectType gameObjectType)
    {
        VisualElement box = new();

        Sprite sprite = prefab.GetComponent<SpriteRenderer>().sprite;

        Image image = new() { sprite = sprite };
        image.style.backgroundColor = Color.gray;
        image.style.paddingTop = 2;
        image.style.paddingBottom = 2;
        image.style.paddingLeft = 2;
        image.style.paddingRight = 2;
        image.style.height = settings.previewSize.y;
        image.style.width = settings.previewSize.x;
        box.Add(image);

        Label label = new() { text = prefab.name };
        label.style.whiteSpace = WhiteSpace.Normal;
        label.style.fontSize = 10;
        label.AddToClassList("text-align-center");
        box.Add(label);

        box.style.width = settings.previewSize.x;

        box.style.marginLeft = settings.margin.x;
        box.style.marginRight = settings.margin.x;
        box.style.marginBottom = settings.margin.y;
        box.style.marginTop = settings.margin.y;

        box.RegisterCallback<MouseDownEvent>(evt =>
        {
            DragAndDrop.PrepareStartDrag();
            DragAndDrop.StartDrag("Dragging");
            DragAndDrop.objectReferences = new Object[] { prefab };
            m_IsDragPerformed = false;
            m_IsDragging = true;
            m_GameObjectType = gameObjectType;
        });

        box.RegisterCallback<DragUpdatedEvent>(evt =>
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Move;
        });


        rootVisualElement.Q<VisualElement>(container).Add(box);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0270:Use coalesce expression")]
    void OnDragEnd()
    {
        if (Event.current.type == EventType.DragPerform)
        {
            m_IsDragPerformed = true;
        }

        if (Event.current.type != EventType.DragExited)
            return;

        if (!m_IsDragging && !m_IsDragPerformed)
            return;

        m_IsDragging = false;
        m_IsDragPerformed = false;

        GameObject go = Selection.activeGameObject;

        string parentTransformName = m_GameObjectType switch
        {
            GameObjectType.Enemy => placerSettings.enemyTransformName,
            GameObjectType.NPC => placerSettings.NPCTransformName,
            GameObjectType.Other => placerSettings.otherTransformName,
            _ => throw new System.NotImplementedException()
        };

        Transform parentTransform = go.transform.Find("/" + parentTransformName);
        if (parentTransform == null) parentTransform = new GameObject(parentTransformName).transform;
        go.transform.SetParent(parentTransform.transform);

        if (m_GameObjectType == GameObjectType.NPC)
        {
            go.name = go.name + "(" + go.GetInstanceID() + ")";
            BehaviorTree behaviorTree = ScriptableObject.CreateInstance<BehaviorTree>();
            AssetDatabase.CreateAsset(behaviorTree, $"Assets/Prefabs/NPCs/Trees/{go.name}.asset");
            AssetDatabase.SaveAssets();
            Npc npc = go.GetComponent<Npc>();
            npc.behaviorTree = behaviorTree;

            EditorUtility.SetDirty(npc);
        }
    }


}