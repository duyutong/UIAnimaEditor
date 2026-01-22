using System;
using System.Collections;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class PersistentData
{
    public UnityEngine.Object target;   // 目标 GameObject
    public string assemblyTypeName;     // 类名
    public string methodName;           // 方法名

    public BTTargetObject btTargetObject;
    public void SerializeBTTargetObject()
    {
        if (btTargetObject == null) btTargetObject = new BTTargetObject();
        btTargetObject.target = target;
        btTargetObject.pathType = EFindObjPathType.ScenePath;
        btTargetObject.SerializeSelf();
        target = null;
    }
}

[Serializable]
public class BTTargetEvent
{
    [SerializeField]
    [HideInInspector]
    public PersistentData[] persistentDatas;

    [SerializeField]
    public UnityEvent targetEvent;

    [NonSerialized]
    public BTRuntime runtime;
    public void SetTargetEvent()
    {
        if (targetEvent != null && targetEvent.GetPersistentEventCount() > 0) return;
        if (persistentDatas == null) return;

        for (int i = 0; i < persistentDatas.Length; i++)
        {
            int index = i;
            PersistentData persistentData = persistentDatas[index];
            BTTargetObject btTargetObject = persistentData.btTargetObject;
            if (btTargetObject == null) continue;
            btTargetObject.runtime = runtime;
            btTargetObject.SetObejctByPath();
            persistentData.target = btTargetObject.target;
            IntegrateEventInfo(index);
        }
    }
    public void SerializeSelf()
    {
        if (targetEvent != null && targetEvent.GetPersistentEventCount() > 0)
        {
            int persistentEventCount = targetEvent.GetPersistentEventCount();
            persistentDatas = new PersistentData[persistentEventCount];
            for (int i = 0; i < persistentEventCount; i++)
            {
                int index = i;
                PersistentData persistentData = ExtractEventInfo(index);
                persistentData.SerializeBTTargetObject();
                persistentDatas.SetValue(persistentData, index);
            }

            targetEvent.RemoveAllListeners();
            targetEvent = null;
        }
    }
    private void IntegrateEventInfo(int index)
    {
        PersistentData persistentData = persistentDatas[index];
        if (persistentData.target == null) return;

        if (targetEvent == null) targetEvent = new UnityEvent();
        // 获取 UnityEventBase 内部的 m_PersistentCalls
        var baseType = typeof(UnityEventBase); // UnityEventBase 是 UnityEvent 的基类
        var callsField = baseType.GetField("m_PersistentCalls", BindingFlags.NonPublic | BindingFlags.Instance);
        var persistentCalls = callsField.GetValue(targetEvent);

        // 获取 PersistentCallGroup 中的 m_Calls 列表
        var callListType = persistentCalls.GetType();
        var callsFieldInGroup = callListType.GetField("m_Calls", BindingFlags.NonPublic | BindingFlags.Instance);

        // 使用反射获取 List<PersistentCall>
        var calls = callsFieldInGroup.GetValue(persistentCalls) as IList;

        // 获取指定索引的 PersistentCall
        Type persistentCallType = Type.GetType("UnityEngine.Events.PersistentCall,UnityEngine.CoreModule");
        var persistentCall = Activator.CreateInstance(persistentCallType);

        // 通过反射获取 PersistentCall 的相关字段
        FieldInfo targetAssemblyTypeNameField = persistentCallType.GetField("m_TargetAssemblyTypeName", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo target = persistentCallType.GetField("m_Target", BindingFlags.NonPublic | BindingFlags.Instance);
        FieldInfo methodName = persistentCallType.GetField("m_MethodName", BindingFlags.NonPublic | BindingFlags.Instance);
        //赋值
        targetAssemblyTypeNameField.SetValue(persistentCall, persistentData.assemblyTypeName);
        UnityEngine.Object targetComponent = persistentData.target.GameObject().GetComponent(persistentData.assemblyTypeName);

        if (targetComponent == null) return;
        target.SetValue(persistentCall, targetComponent);
        methodName.SetValue(persistentCall, persistentData.methodName);

        calls.Insert(index, persistentCall);

        callsFieldInGroup.SetValue(persistentCalls, calls);
    }
    private PersistentData ExtractEventInfo(int index)
    {
        // 获取 UnityEventBase 内部的 m_PersistentCalls
        var baseType = typeof(UnityEventBase); // UnityEventBase 是 UnityEvent 的基类
        var callsField = baseType.GetField("m_PersistentCalls", BindingFlags.NonPublic | BindingFlags.Instance);
        if (callsField == null)
        {
            Debug.LogError("Could not find m_PersistentCalls field in UnityEventBase.");
            return null;
        }

        var persistentCalls = callsField.GetValue(targetEvent);
        if (persistentCalls == null)
        {
            Debug.LogError("m_PersistentCalls is null.");
            return null;
        }

        // 获取 PersistentCallGroup 中的 m_Calls 列表
        var callListType = persistentCalls.GetType();
        var callsFieldInGroup = callListType.GetField("m_Calls", BindingFlags.NonPublic | BindingFlags.Instance);
        if (callsFieldInGroup == null)
        {
            Debug.LogError("Could not find m_Calls field in PersistentCallGroup.");
            return null;
        }

        // 使用反射获取 List<PersistentCall>
        var calls = callsFieldInGroup.GetValue(persistentCalls) as IList;
        if (calls == null || calls.Count <= index)
        {
            Debug.LogError("Index out of range or no PersistentCalls available.");
            return null;
        }

        // 获取指定索引的 PersistentCall
        var persistentCall = calls[index];

        // 通过反射获取 PersistentCall 的相关字段
        Type persistentCallType = persistentCall.GetType();
        var targetAssemblyTypeNameField = persistentCallType.GetField("m_TargetAssemblyTypeName", BindingFlags.NonPublic | BindingFlags.Instance);

        if (targetAssemblyTypeNameField == null)
        {
            Debug.LogError("Failed to retrieve necessary fields from PersistentCall.");
            return null;
        }

        // 获取值并赋值给目标变量
        PersistentData data = new PersistentData();
        data.target = targetEvent.GetPersistentTarget(index);
        data.methodName = targetEvent.GetPersistentMethodName(index);
        data.assemblyTypeName = (targetAssemblyTypeNameField.GetValue(persistentCall) as string).Split(",")[0];

        return data;
    }
}
