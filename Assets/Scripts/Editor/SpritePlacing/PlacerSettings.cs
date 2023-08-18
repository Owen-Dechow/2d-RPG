using UnityEditor;
using UnityEngine;

[CreateAssetMenu()]
public class PlacerSettings : ScriptableObject
{
    public Vector2 previewSize;
    public Vector2 margin;
    public GameObject[] NPCs;
    public GameObject[] enemies;
    public GameObject[] other;
    public string NPCTransformName;
    public string enemyTransformName;
}