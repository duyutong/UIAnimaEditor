using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class BehaviorTreeBaseState
{
    public EBTState state = EBTState.未开始;
    public string stateName = "*BehaviorTreeBaseState";
    public string nodeId;
    public bool interruptible = false;
    public string interruptTag = "";
    public BTRuntime runtime;
    public List<BTOutputInfo> output = new List<BTOutputInfo>();
    public List<BehaviorTreeBaseState> lastStates;
    public virtual BTStateObject stateObj { get; }

    public Action onEnterForRuntime;
    public Action onExecuteForRuntime;
    public Action onExitForRuntime;
    /// <summary>
    /// 初始化参数
    /// </summary>
    /// <param name="param">参数</param>
    public virtual void InitParam(string param) {}
    public void InitBTTarget() 
    {
        Type type = stateObj.GetType();
        if (type == null) return;
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            if (field.FieldType == typeof(BTTargetObject))
            {
                BTTargetObject bTTargetObject = (BTTargetObject)field.GetValue(stateObj);
                if (bTTargetObject != null) 
                {
                    bTTargetObject.runtime = runtime;
                    bTTargetObject.SetObejctByPath();
                }
            }
            if (field.FieldType == typeof(BTTargetEvent)) 
            {
                BTTargetEvent bTTargetEvent = (BTTargetEvent)field.GetValue(stateObj);
                if (bTTargetEvent != null) 
                {
                    bTTargetEvent.runtime = runtime;
                    bTTargetEvent.SetTargetEvent();
                }
            }
            if (field.FieldType == typeof(BTTargetContainer)) 
            {
                BTTargetContainer bTTargetContainer= (BTTargetContainer)field.GetValue(stateObj);
                if (bTTargetContainer != null)
                {
                    bTTargetContainer.runtime = runtime;
                    bTTargetContainer.SetContainer();
                }
            }
        }
    }

    public void InitValueReflact()
    {
        if (runtime.lastStateDic[nodeId].Count == 0) return;

        Type type = GetType();
        foreach (BehaviorTreeBaseState lastState in lastStates)
        {
            foreach (BTOutputInfo inputInfo in lastState.output)
            {
                if (inputInfo.value == null) continue;
                bool isCommValue = string.IsNullOrEmpty(inputInfo.nodeId);
                if (!isCommValue && nodeId != inputInfo.nodeId) continue;
                string memberName = inputInfo.toPortName;
                FieldInfo fieldInfo = type.GetField(memberName);
                if (fieldInfo == null) continue;
                fieldInfo.SetValue(this, inputInfo.value);
            }
        }
    }

    public void InitValue()
    {
        if (SetFieldValue() == ESetFieldValueResult.None)
        { 
            InitValueReflact(); 
            return;
        }

        foreach (BehaviorTreeBaseState lastState in lastStates) 
        {
            foreach (BTOutputInfo inputInfo in lastState.output) 
            {
                if (inputInfo.value == null) continue;
                bool isCommValue = string.IsNullOrEmpty(inputInfo.nodeId);
                if (!isCommValue && nodeId != inputInfo.nodeId) continue;
                string fieldName = inputInfo.toPortName;
                object value = inputInfo.value;

                // 如果子类没有实现，父类使用默认的赋值逻辑
                ESetFieldValueResult result = SetFieldValue(fieldName, value);
                if (result == ESetFieldValueResult.Fail) continue;
            }
        }
         
    }
    /// <summary>
    /// 默认的字段赋值逻辑。可由子类重写以实现具体的赋值操作。
    /// </summary>
    /// <param name="fieldName">字段名称。</param>
    /// <param name="value">要设置的字段值。</param>
    /// <returns>
    /// 返回 <see cref="ESetFieldValueResult.Succ"/> 表示成功处理该字段；
    /// 返回 <see cref="ESetFieldValueResult.Fail"/> 表示字段无法处理；
    /// 返回 <see cref="ESetFieldValueResult.None"/> 表示未进行任何操作（默认行为）。
    /// </returns>
    protected virtual ESetFieldValueResult SetFieldValue(string fieldName = default, object value = default)
    {
        // 默认实现：不处理任何字段，返回 None
        return ESetFieldValueResult.None;
    }
    /// <summary>
    /// 保存状态
    /// </summary>
    public virtual void Save() { }
    public virtual void OnInitLastStates() 
    {
        if (lastStates == null)
        {
            lastStates = new List<BehaviorTreeBaseState>();
            List<string> lastStateIds = runtime.lastStateDic[nodeId];
            foreach (string id in lastStateIds) lastStates.Add(runtime.stateDic[id]);
        }
    }
    public virtual void OnInitFinish(){ }

    /// <summary>
    /// 每帧无条件调用的函数，表示这个节点正在被检测
    /// </summary>
    public virtual void OnCheck() { }
    /// <summary>
    /// 进入状态回调
    /// </summary>
    public virtual void OnEnter() 
    {
        InitValue();
        state = EBTState.进入;
        onEnterForRuntime?.Invoke();
    }

    /// <summary>
    /// 执行状态回调
    /// </summary>
    public virtual void OnExecute() { state = EBTState.执行中; onExecuteForRuntime?.Invoke(); }

    /// <summary>
    /// 退出状态回调
    /// </summary>
    public virtual void OnExit() 
    {
        if (output.Count == 1 && output[0].fromPortName == "exit") output[0].value = true;

        state = EBTState.完成;
        onExitForRuntime?.Invoke();
    }
    /// <summary>
    /// 打断
    /// </summary>
    public virtual void OnInterrupt() { state = EBTState.中断; }
    /// <summary>
    /// 恢复打断状态
    /// </summary>
    public virtual void OnResume() { state = EBTState.执行中; }
    /// <summary>
    /// 刷新状态回调
    /// </summary>
    public virtual void OnRefresh()
    {
        state = EBTState.未开始;
        ClearOutputList();
    }

    /// <summary>
    /// 清除输出列表
    /// </summary>
    public virtual void ClearOutputList()
    {
        for (int i = 0; i < output.Count; i++)
        {
            BTOutputInfo info = output[i];
            if (info.value == null) continue;
            info.value = null;
            output[i] = info;
        }
    }
    public List<BehaviorTreeBaseState> GetNextStates() 
    {
        List<BehaviorTreeBaseState> nextStates = new List<BehaviorTreeBaseState>();
        List<string> stateIds = new List<string>();
        foreach (KeyValuePair<string, List<string>> keyValuePair in runtime.lastStateDic)
        {
            string stateId = keyValuePair.Key;
            if (stateId == nodeId) continue;
            List<string> values = keyValuePair.Value;
            if (!values.Contains(nodeId)) continue;
            if (stateIds.Contains(stateId)) continue;
            stateIds.Add(stateId);
        }

        foreach (string _id in stateIds)
        {
            nextStates.Add(runtime.stateDic[_id]);
        }
        return nextStates;
    }
    /// <summary>
    /// 获取上一个符合条件的状态
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    /// <param name="selectFunc">选择条件函数</param>
    /// <returns>符合条件的状态</returns>
    public T GetLastMatchingState<T>(Func<T, bool> selectFunc) where T : BehaviorTreeBaseState
    {
        T result = this as T;
        if (result != null && selectFunc(result)) return result;
        else
        {
            foreach (BehaviorTreeBaseState _state in lastStates)
            {
                if (_state.GetLastMatchingState(selectFunc) == null) continue;
                else return _state.GetLastMatchingState(selectFunc);
            }
        }
        return result;
    }
    /// <summary>
    /// 将操作传递到前置节点，直到检查到的节点符合条件
    /// 遇到条件函数返回为true的节点后会先对该节点执行行为函数，然后结束传递
    /// </summary>
    /// <param name="action">操作函数</param>
    /// <param name="checkFunc">检查条件函数</param>
    public void Infect(Action<BehaviorTreeBaseState> action, Func<BehaviorTreeBaseState, bool> checkFunc)
    {
        action(this);
        if (checkFunc(this)) return;
        foreach (BehaviorTreeBaseState _state in lastStates)
        {
            _state.Infect(action, checkFunc);
        }
    }
    /// <summary>
    /// 获取 AnimationCurve 的开始时间和结束时间
    /// </summary>
    /// <param name="curve">需要检查的 AnimationCurve</param>
    /// <returns>返回一个元组，包含曲线的开始时间和结束时间</returns>
    public (float startTime, float endTime) GetCurveTimeRange(AnimationCurve curve)
    {
        // 如果曲线为空或没有关键帧，返回默认值
        if (curve == null || curve.length == 0) return (0f, 0f);

        // 获取关键帧数组
        Keyframe[] keys = curve.keys;
        // 获取第一个关键帧的时间作为开始时间
        float startTime = keys[0].time;
        // 获取最后一个关键帧的时间作为结束时间
        float endTime = keys[keys.Length - 1].time;

        return (startTime, endTime);
    }

    /// <summary>
    /// 更新状态回调
    /// </summary>
    public virtual void OnUpdate()
    {
        if (state != EBTState.执行中) goto wait;
        wait:;
    }
}

public enum ESetFieldValueResult 
{
    Succ,
    Fail,
    None,
}
[Serializable]
public enum EBTState
{
    未开始,
    进入,
    执行中,
    完成,
    中断,
    空,       //当作为随机分支时，状态为空，代表被放弃的分支
}

[Serializable]
public class BTOutputInfo
{
    public string fromPortName;
    public string toPortName;
    public object value;
    public string nodeId;
}
