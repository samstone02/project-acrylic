using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Tank))]
public class TankEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Tank tank = (Tank)target;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Take Damage"))
        {
            tank.TakeDamageRpc(1);
        }
        EditorGUILayout.EndHorizontal();
        DrawDefaultInspector();
    }
}