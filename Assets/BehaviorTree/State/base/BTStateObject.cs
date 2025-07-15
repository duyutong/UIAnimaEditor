using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTStateObject : ScriptableObject
{
    public bool interruptible = false;
    public string interruptTag = "";

    public List<BTOutputInfo> output = new List<BTOutputInfo>();
    /// <summary>
    /// 刷新输出
    /// </summary>
    /// <param name="newInfo">新信息</param>
    /// <param name="isRemove">是否移除</param>
    public void RefreshOutput(BTOutputInfo newInfo, bool isRemove)
    {
        if (isRemove)
        {
            for (int i = output.Count - 1; i >= 0; i--)
            {
                BTOutputInfo info = output[i];
                if (info.nodeId != newInfo.nodeId) continue;
                if (info.fromPortName != newInfo.fromPortName) continue;
                if (info.toPortName != newInfo.toPortName) continue;
                output.Remove(info);
            }
        }
        else
            output.Add(newInfo);
    }
}
public class BTTiggerStateObject : BTStateObject 
{
    public string triggerTag = "";
}
