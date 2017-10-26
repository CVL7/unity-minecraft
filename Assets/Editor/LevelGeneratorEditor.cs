/*using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var levelGenerator = (LevelGenerator) target;
        DrawDefaultInspector();
        
        if (GUILayout.Button("Generate"))
        {
            levelGenerator.Generate();
        }

        if (GUILayout.Button("Clean"))
        {
            levelGenerator.CleanLevel();
        }
        serializedObject.ApplyModifiedProperties();
    }
}
*/