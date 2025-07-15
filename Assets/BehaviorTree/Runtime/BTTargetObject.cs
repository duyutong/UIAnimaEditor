using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class BTTargetObject
{
    public UnityEngine.Object target;
    public string scenePath;
    public string localPath;
    public EFindObjPathType pathType;

    private string selfPath = "self";

    [NonSerialized]
    public BTRuntime runtime;
    public void SetObejctByPath()
    {
        if (target != null) return;

        if (pathType == EFindObjPathType.ScenePath) 
        {
            //if (runtime == null) return;
            if(string.IsNullOrEmpty(scenePath)) return;
            target = GameObject.Find(scenePath);
        }
        if (pathType == EFindObjPathType.LocalPath)
        {
            if (runtime == null) return;
            if (runtime.gameObject == null) return;
            if (runtime.transform == null) return;
            if (string.IsNullOrEmpty(localPath)) return;

            if (localPath == selfPath) target = runtime.gameObject;
            else target = runtime.transform.Find(localPath);
        }
    }
    public void SerializeSelf()
    {
        scenePath = string.Empty;

        if (pathType == EFindObjPathType.ScenePath) scenePath = GetObjPath(target);
        if (pathType == EFindObjPathType.LocalPath) 
        {
            if(target!=null) localPath = GetObjPath(target); 
        }

        target = null;
    }
    private string GetObjPath(UnityEngine.Object obj)
    {
        GameObject gameObject = null;

        if (obj is GameObject)
        {
            gameObject = obj as GameObject;
        }
        else if (obj is Component)
        {
            gameObject = (obj as Component).gameObject;
        }

        if (gameObject != null)
        {
            if (pathType == EFindObjPathType.ScenePath) return GetScenePath(gameObject);
            if (pathType == EFindObjPathType.LocalPath) return GetLoaclPath(gameObject);
            return string.Empty;
        }
        else
        {
            Debug.LogWarning("The provided object is not a GameObject or Component.");
            return string.Empty;
        }
    }
    private string GetLoaclPath(GameObject obj)
    {
        if (obj == null)
        {
            return string.Empty;
        }
        string path = string.Empty;
        Transform current = obj.transform;

        if (current.GetComponent<BTRuntimeComponent>() != null) return selfPath; 
        
        while (current!=null && current.GetComponent<BTRuntimeComponent>() == null)
        {
            path = path + "/" + current.name;

            if (current == null) break;
            else current = current.parent;
        }
        path = path.Substring(1, path.Length - 1);
        return path;
    }
    private string GetScenePath(GameObject obj)
    {
        if (obj == null)
        {
            return string.Empty;
        }

        string path = obj.name;

        Transform current = obj.transform;

        while (current.parent != null)
        {
            current = current.parent;
            path = current.name + "/" + path;
        }

        return path;
    }
}
public enum EFindObjPathType
{
    LocalPath,
    ScenePath,
}
