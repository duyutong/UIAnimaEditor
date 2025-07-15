using System.Diagnostics;
using UnityEditor;
using UnityEngine.UIElements;

public class InspectorView : VisualElement
{
    public Editor editor;
    public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }
    internal void UpdateSelection(BehaviorTreeBaseNode node) 
    {
        Clear();
        UnityEngine.Object.DestroyImmediate(editor);
        if (node.btState.stateObj == null) return;
        UnityEngine.Debug.Log(node.guid);
        node.btState.Save();
        editor = Editor.CreateEditor(node.btState.stateObj);
        IMGUIContainer container = new IMGUIContainer(() =>
        {
            if (node == null || node.btState == null) return;
            editor.OnInspectorGUI();
        });
        Add(container);
    }
}

