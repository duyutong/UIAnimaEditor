using D.Unity3dTools;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using BTState = BehaviorTreeBaseState;

public class BTRuntimeComponent : MonoBehaviour
{
    [SerializeField]
    public BTContainer container;
    public Dictionary<string, BTState> stateDic = new Dictionary<string, BTState>();
    public Dictionary<string, List<string>> lastStateDic = new Dictionary<string, List<string>>();

    private List<string> statesIds = new List<string>();
    private bool isInitFinish;
    private int componentIndex;
    private void Awake()
    {
        isInitFinish = false;
        LoadStates();
        BTRuntimeController.AddRuntime(this, (_index) => { componentIndex = _index; });
    }
    /// <summary>
    /// 加载行为树状态
    /// </summary>
    private void LoadStates()
    {
        foreach (NodeData nodeData in container.nodeDatas)
        {
            Type stateType = Type.GetType(nodeData.stateName);
            BTState btState = (BTState)Activator.CreateInstance(stateType);//做一个池子会更好
            btState.stateName = nodeData.stateName;
            btState.runtime = this;
            btState.nodeId = nodeData.guid;
            btState.InitParam(nodeData.stateParams);
            btState.InitBTTargetObejct();
            stateDic.Add(nodeData.guid, btState);
            lastStateDic.Add(nodeData.guid, nodeData.lastNodes);
        }
        foreach (KeyValuePair<string, BTState> keyValuePair in stateDic)
        {
            BTState state = keyValuePair.Value;
            state.OnInitLastStates();
        }
        foreach (KeyValuePair<string, BTState> keyValuePair in stateDic)
        {
            BTState state = keyValuePair.Value;
            state.OnInitFinish();
        }
        //进行拓扑排序
        List<TopologyData> topologyDatas = new List<TopologyData>();
        foreach (KeyValuePair<string, BTState> keyValuePair in stateDic) 
        {
            BTState state = keyValuePair.Value;
            TopologyData topologyData = new TopologyData();
            topologyData.inDegree = new List<string>();
            topologyData.outDegree = new List<string>();
            //标记自己的id
            topologyData.nodeId = state.nodeId;
            //给入度赋值
            foreach (string _input in lastStateDic[state.nodeId])
                topologyData.inDegree.Add(_input);
            //给出度赋值
            foreach (BTOutputInfo outputInfo in state.output)
                topologyData.outDegree.Add(outputInfo.nodeId);

            topologyDatas.Add(topologyData);
        }
        statesIds = TopologicalSort(topologyDatas);
        isInitFinish = true;
    }
    public List<string> TopologicalSort(List<TopologyData> nodes)
    {
        // 创建一个字典来存储每个节点的入度
        Dictionary<string, int> inDegree = new Dictionary<string, int>();
        // 创建一个字典来存储节点对象
        Dictionary<string, TopologyData> nodeDict = new Dictionary<string, TopologyData>();

        // 初始化字典
        foreach (var node in nodes)
        {
            nodeDict[node.nodeId] = node;
            inDegree[node.nodeId] = node.inDegree.Count;
        }

        // 创建一个队列来存储入度为0的节点
        Queue<string> queue = new Queue<string>();
        foreach (var node in nodes)
        {
            if (inDegree[node.nodeId] == 0)
            {
                queue.Enqueue(node.nodeId);
            }
        }

        // 创建一个列表来存储排序后的节点顺序
        List<string> sortedList = new List<string>();

        while (queue.Count > 0)
        {
            var nodeId = queue.Dequeue();
            sortedList.Add(nodeId);

            foreach (var output in nodeDict[nodeId].outDegree)
            {
                inDegree[output]--;
                if (inDegree[output] == 0)
                {
                    queue.Enqueue(output);
                }
            }
        }

        // 检查是否存在循环依赖
        if (sortedList.Count != nodes.Count)
        {
            throw new InvalidOperationException("Graph has at least one cycle, topological sort not possible.");
        }

        return sortedList;
    }

    /// <summary>
    /// 设置行为树状态值
    /// </summary>
    /// <param name="action">要执行的动作</param>
    /// <param name="func">要应用的条件</param>
    public void SetStateValue(Action<BTState> action, Func<BTState, bool> func)
    {
        foreach (KeyValuePair<string, BTState> keyValuePair in stateDic)
        {
            BTState state = keyValuePair.Value;
            if (!func(state)) continue;
            action(state);
        }
    }
    public void TriggerBtStateAndRun(string triggerTag) 
    {
        OnReceiveMsg(triggerTag, EBTState.进入);
    }
    public void ResetStateAndRun() 
    {
        isInitFinish = false;
        foreach (KeyValuePair<string, BTState> keyValuePair in stateDic) 
        {
            BTState state = keyValuePair.Value;
            state.OnRefresh();
        }
        isInitFinish = true;
    }
    public void OnReceiveMsg(string stateTag, EBTState eBTState)
    {
        Debug.Log($"stateTag {stateTag} eBTState {eBTState}");

        foreach (KeyValuePair<string, BTState> keyValuePair in stateDic)
        {
            BTState checkState = keyValuePair.Value;
            if (eBTState == EBTState.中断)
            {
                if (checkState.state != EBTState.执行中) continue;
                if (!checkState.interruptible) continue;
                if (checkState.interruptTag != stateTag) continue;
                checkState.OnInterrupt();
            }
            else if (eBTState == EBTState.进入) 
            {
                TiggerBaseState check = checkState as TiggerBaseState;
                bool isTrigger = check != null && !string.IsNullOrEmpty(check.triggerTag);
                if (!isTrigger) continue;
                if (check.triggerTag != stateTag) continue;
                if (checkState.state != EBTState.未开始) checkState.OnRefresh();
                checkState.OnEnter();
            }
        }
    }
    private bool CheckIsLogicGate(BTState checkState) 
    {
        return (checkState as LogicGateState) != null;
    }
    private bool CheckIsTrigger(BTState checkState) 
    {
        TiggerBaseState check = checkState as TiggerBaseState;
        return check != null && !string.IsNullOrEmpty(check.triggerTag);
    }
    // Update is called once per frame
    void Update()
    {
        if (!isInitFinish) return;   
        foreach (string nodeId in statesIds)
        {
            BTState checkState = stateDic[nodeId];
            bool isLogicGate = CheckIsLogicGate(checkState);
            bool isTrigger = CheckIsTrigger(checkState);
            bool isChanRunAuto = !isTrigger && lastStateDic[nodeId].Count == 0 && checkState.state == EBTState.未开始;
            if (isChanRunAuto)
            {
                checkState.OnEnter();
            }
            else if (checkState.state == EBTState.未开始)
            {
                int checkCount = lastStateDic[nodeId].Count;
                bool isExistFinish = false;
                foreach (string lastStateId in lastStateDic[nodeId])
                {
                    BTState lastState = stateDic[lastStateId];
                    if (lastState.state == EBTState.完成) { checkCount--; isExistFinish = true; }
                    if ((isLogicGate && isExistFinish) || checkCount == 0) 
                    {
                        checkState.OnEnter();
                    }  
                }
            }
            else if (checkState.state == EBTState.执行中)
            {
                checkState.OnUpdate();
            }
        }
    }
    private void OnDestroy()
    {
        BTRuntimeController.RemoveRuntime(componentIndex);
    }
}
public class TopologyData
{
    public string nodeId;
    public List<string> inDegree;
    public List<string> outDegree;
}
