using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Badges.Equip))]
public class BadgeEditor : PropertyDrawer
{
    enum ExtraBadgeKind
    {
        Defense,
        Attack,
    }

    int subPNumber;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty defense = property.FindPropertyRelative("defense");
        SerializedProperty attack = property.FindPropertyRelative("attack");
        SerializedProperty special = property.FindPropertyRelative("special");
        SerializedProperty extra1Kind = property.FindPropertyRelative("extra1Kind");
        SerializedProperty extra2Kind = property.FindPropertyRelative("extra2Kind");
        SerializedProperty extraAttack1 = property.FindPropertyRelative("extraAttack1");
        SerializedProperty extraAttack2 = property.FindPropertyRelative("extraAttack2");
        SerializedProperty extraDefense1 = property.FindPropertyRelative("extraDefense1");
        SerializedProperty extraDefense2 = property.FindPropertyRelative("extraDefense2");

        Rect foldOutBox = new(position.min.x, position.min.y, position.size.x, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldOutBox, property.isExpanded, label);
        if (!property.isExpanded)
        {
            EditorGUI.EndProperty();
            return;
        }

        subPNumber = 0;
        DrawProperty(defense, position);
        DrawProperty(attack, position);
        DrawProperty(special, position);

        subPNumber++;
        DrawProperty(extra1Kind, position);
        if (extra1Kind.enumValueIndex == 0) DrawProperty(extraAttack1, position);
        else DrawProperty(extraDefense1, position);

        subPNumber++;
        DrawProperty(extra2Kind, position);
        if (extra2Kind.enumValueIndex == 0) DrawProperty(extraAttack2, position);
        else DrawProperty(extraDefense2, position);

        EditorGUI.EndProperty();
    }
    public void DrawProperty(SerializedProperty property, Rect position)

    {
        subPNumber++;
        Rect newPosition = position;
        newPosition.y = position.y + (EditorGUIUtility.singleLineHeight) * subPNumber;
        newPosition.size = new Vector2(newPosition.size.x, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(newPosition, property);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.isExpanded) { return 10 * EditorGUIUtility.singleLineHeight; }
        else return EditorGUIUtility.singleLineHeight;
    }
}
