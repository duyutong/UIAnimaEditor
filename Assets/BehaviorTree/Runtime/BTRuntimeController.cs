using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BTRuntimeController : MonoBehaviour
{
    public static BTRuntimeController ins { get { return _ins; } }
    private static BTRuntimeController _ins;
    private static Dictionary<int, BTRuntimeComponent> bTRuntimes = new Dictionary<int, BTRuntimeComponent>();
    private void OnEnable()
    {
        _ins = this;
    }
    public static void AddRuntime(BTRuntimeComponent bTRuntime, Action<int> action)
    {
        int index = bTRuntime.GetInstanceID();
        bTRuntimes.Add(index, bTRuntime);
        action(index);
    }
    public static void RemoveRuntime(int index)
    {
        if (!bTRuntimes.ContainsKey(index)) return;
        bTRuntimes.Remove(index);
    }
    public void SendToTag(string _tag, EBTState eBTState)
    {
        foreach (KeyValuePair<int, BTRuntimeComponent>  keyValuePair in bTRuntimes)
        {
            BTRuntimeComponent bTRuntime = keyValuePair.Value;
            if (bTRuntime == null) continue;
            bTRuntime.OnReceiveMsg(_tag, eBTState);
        }
    }
}
