using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TankMotionAudioController))]
public class TankMotionAudioControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var audioController = (TankMotionAudioController)target;

        if (GUILayout.Button("Start Engine"))
        {
            audioController.StartEngine();
        }
        DrawDefaultInspector();
    }
}