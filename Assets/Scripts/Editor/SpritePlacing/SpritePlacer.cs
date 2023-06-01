using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


public class SpritePlacer : EditorWindow
{
    PlacerSettings placerSettings;
    bool m_IsDragPerformed = false;
    bool m_IsDragging = false;
    bool m_NPC;

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
            CreateDragAndDrop(prefab, placerSettings, "NPCs", true);
        }
        foreach (GameObject prefab in placerSettings.enemies)
        {
            CreateDragAndDrop(prefab, placerSettings, "enemies", false);
        }

        SceneView.duringSceneGui += sv => OnDragEnd();
        EditorApplication.hierarchyWindowItemOnGUI += (id, rect) => OnDragEnd();
    }

    void CreateDragAndDrop(GameObject prefab, PlacerSettings settings, string container, bool unpackAfterDrag)
    {
        VisualElement box = new();
        

        Sprite sprite = prefab.GetComponent<SpriteRenderer>().sprite;

        Image image = new() { sprite=sprite };
        image.style.height = settings.previewSize.y;
        image.style.width = settings.previewSize.x;
        box.Add(image);
        
        box.style.width = settings.previewSize.x;
        box.style.height = settings.previewSize.y;

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
            m_NPC = unpackAfterDrag;
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

        if (Event.current.type == EventType.DragExited)
        {
            if (m_IsDragging && m_IsDragPerformed)
            {
                m_IsDragging = false;
                m_IsDragPerformed = false;

                var go = Selection.activeGameObject;


                string parentTransformName = m_NPC ? placerSettings.NPCTransform : placerSettings.enemyTransform;
                Transform parentTransform = go.transform.Find("/" + parentTransformName);
                if (parentTransform == null) parentTransform = new GameObject(parentTransformName).transform;
                go.transform.SetParent(parentTransform.transform);

                if (m_NPC)
                {
                    go.name = go.name + "(" + go.GetInstanceID() + ")";
                    BehaviourTree behaviourTree = ScriptableObject.CreateInstance<BehaviourTree>();
                    AssetDatabase.CreateAsset(behaviourTree, $"Assets/Prefabs/NPCs/Trees/{go.name}.asset");
                    AssetDatabase.SaveAssets();
                    go.GetComponent<Npc>().behaviorTree = behaviourTree;
                }
            }
        }
    }
}