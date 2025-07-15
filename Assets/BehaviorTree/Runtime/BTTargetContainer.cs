using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[Serializable]
public class BTTargetContainer
{
    public BTContainer target;
    public string assetPath;

    [NonSerialized]
    public BTRuntime runtime;

    private Action<bool> loadFinish;
    public void SetContainer(Action<bool> _loadFinish = null) 
    {
        if (target != null) { _loadFinish?.Invoke(true); return; }

#if UNITY_EDITOR
        target = AssetDatabase.LoadAssetAtPath<BTContainer>(assetPath);
        _loadFinish?.Invoke(true);
#else
        loadFinish = _loadFinish;
        Addressables.LoadAssetAsync<BTContainer>(assetPath).Completed += OnLoadDone;
#endif
    }
    private void OnLoadDone(AsyncOperationHandle<BTContainer> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded) target = handle.Result;
        else Debug.LogError("º”‘ÿ ß∞‹£∫" + assetPath);

        loadFinish?.Invoke(handle.Status == AsyncOperationStatus.Succeeded);
        loadFinish = null;
    }
    public void SerializeSelf() 
    {
#if UNITY_EDITOR
        assetPath = null;
        if (target == null) return;
        assetPath = AssetDatabase.GetAssetPath(target);
        target = null;
#endif
    }
}
