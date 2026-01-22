using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class MyCustomAssetHandler : AssetPostprocessor
{
    [OnOpenAsset]
    public static bool OnOpenBTAsset(int instanceID, int line) 
    {
        // 获取被双击的资产
        Object obj = EditorUtility.InstanceIDToObject(instanceID);
        BTContainer container = obj as BTContainer;
        if (container!=null) 
        {
            BehaviourTreeEditor editor = BehaviourTreeEditor.OpenWindow();
            editor.OpenBTAsset(container);
            return true;
        }
        return false;
    }
}
