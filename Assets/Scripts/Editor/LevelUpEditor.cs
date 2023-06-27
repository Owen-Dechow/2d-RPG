using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelUp))]
public class LevelUpEditor : Editor
{
    public static int sliderPos;

    public override void OnInspectorGUI()
    {
        LevelUp levelUp = target as LevelUp;

        CurveField(ref levelUp.expNeeded, "Exp Needed", 3, 10_000);
        CurveField(ref levelUp.life, "Max Life", 5, 500);
        CurveField(ref levelUp.magic, "Max Magic", 5, 250);
        CurveField(ref levelUp.attack, "Attack", 1, 100);
        CurveField(ref levelUp.defense, "Defense", 1, 100);
        EditorGUILayout.Space(10);

        GUILayout.Label("Stats for level");
        sliderPos = EditorGUILayout.IntSlider(sliderPos, 1, 100);

        string text = "";
        LevelUp.LevelData levelData = levelUp.GetDataForLevel(sliderPos);

        text += $"Exp Needed:\t {(int)levelUp.expNeeded.Evaluate(sliderPos):n0}\n";
        text += $"Max Life:\t {levelData.life}\n";
        text += $"Max Magic:\t {levelData.magic} \n";
        text += $"Attack:\t\t {levelData.attack} \n";
        text += $"Defense:\t {levelData.defense}";
        GUILayout.TextArea(text);

    }

    void CurveField(ref AnimationCurve curve, string label, int minVal, int maxVal)
    {
        if (curve is null)
        {
            curve = new AnimationCurve(new Keyframe[2]);
            return;
        }

        if (curve.length < 2)
        {
            curve = new AnimationCurve(new Keyframe[2]);
            return;
        }

        Keyframe[] keyframes = curve.keys;
        keyframes[0].time = 1;
        keyframes[0].value = minVal;
        keyframes[^1].time = 100;
        keyframes[^1].value = maxVal;
        curve.keys = keyframes;

        float last = 0;
        int error = 0;
        for (int i = 0; i <= 100; i++)
        {
            if (curve.Evaluate(i) < last)
            {
                error = i;
                break;
            }
            last = curve.Evaluate(i);
        }

        if (error > 0)
        {
            EditorGUILayout.CurveField(label + $" | Error at level: {error}", curve, Color.red, new Rect(0, 0, 100, maxVal));
        }
        else
        {
            EditorGUILayout.CurveField(label, curve, Color.green, new Rect(0, 0, 100, maxVal));
        }
    }
}
