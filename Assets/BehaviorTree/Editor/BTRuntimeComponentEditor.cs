using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BTRuntimeComponent))]
public class BTRuntimeComponentEditor : Editor
{
    SerializedProperty btContainerProp;
    private void OnEnable()
    {
        btContainerProp = serializedObject.FindProperty("container");
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        BTRuntimeComponent script = (BTRuntimeComponent)target;// 获取目标对象
        GUILayout.Space(10); // 添加一些空间
        if (GUILayout.Button("OpenRuntimeView", GUILayout.Height(50)))
        {
            // 点击按钮时的操作
            BehaviourTreeEditor editor = BehaviourTreeEditor.OpenWindow();
            script.InitRuntime();
            editor.LoadRuntimeContainer(script.runtime);
        }
    }
}
