using System;
using System.Collections.Generic;
using UnityEngine;
using BTState = BehaviorTreeBaseState;

public class BTRuntime
{
    public BTContainer container;
    public GameObject gameObject;
    public Transform transform;

    public Dictionary<string, BTState> stateDic = new Dictionary<string, BTState>();
    public Dictionary<string, List<string>> lastStateDic = new Dictionary<string, List<string>>();

    private List<string> statesIds = new List<string>();

    [HideInInspector] public bool isInitFinish;
    [HideInInspector] public int runtimeIndex;

    public bool isAllStateExecuteFinish { get; private set; }
    /// <summary>
    /// 加载行为树状态
    /// </summary>
    public void LoadStates()
    {
        foreach (NodeData nodeData in container.nodeDatas)
        {
            Type stateType = Type.GetType(nodeData.stateName);
            BTState btState = BTObjectPool.GetObject<BTState>(stateType);
            btState.stateName = nodeData.stateName;
            btState.runtime = this;
            btState.nodeId = nodeData.guid;
            btState.InitParam(nodeData.stateParams);
            btState.InitBTTarget();
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
    private List<string> TopologicalSort(List<TopologyData> nodes)
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
    public void UnLoadStates()
    {
        foreach (KeyValuePair<string, BTState> keyValuePair in stateDic)
        {
            BTState state = keyValuePair.Value;
            BTObjectPool.ReturnObject(state);
        }
        lastStateDic.Clear();
        stateDic.Clear();
        isInitFinish = false;
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
    public void ResetAllStateAndRun()
    {
        isInitFinish = false;
        foreach (KeyValuePair<string, BTState> keyValuePair in stateDic)
        {
            BTState state = keyValuePair.Value;
            state.OnRefresh();
        }
        isInitFinish = true;
    }
    public void OnReceiveMsg(string triggerTag, EBTState eBTState)
    {
        // Debug.Log($"stateTag {triggerTag} eBTState {eBTState}");

        foreach (KeyValuePair<string, BTState> keyValuePair in stateDic)
        {
            BTState checkState = keyValuePair.Value;
            if (eBTState == EBTState.中断)
            {
                if (checkState.state != EBTState.执行中) continue;
                if (!checkState.interruptible) continue;
                if (checkState.interruptTag != triggerTag) continue;
                checkState.OnInterrupt();
            }
            else if (eBTState == EBTState.进入)
            {
                TiggerBaseState check = checkState as TiggerBaseState;
                bool isTrigger = check != null && !string.IsNullOrEmpty(check.triggerTag);
                if (!isTrigger) continue;
                if (check.triggerTag != triggerTag) continue;
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
    public void OnUpdate()
    {
        if (!isInitFinish) return;
        if (stateDic == null) return;

        isAllStateExecuteFinish = true;
        foreach (string nodeId in statesIds)
        {
            BTState checkState = stateDic[nodeId];
            checkState.OnCheck();

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

                bool isExistFinish = false;//是否存在已经完成的前置状态
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

            if (checkState.state != EBTState.完成) isAllStateExecuteFinish = false;
        }
    }
    public void OnDisable()
    {
        if (isInitFinish)
        {
            UnLoadStates();
            BTRuntimeController.RemoveRuntime(runtimeIndex);
        }
    }
    public void OnEnable()
    {
        if (!isInitFinish)
        {
            LoadStates();
            BTRuntimeController.AddRuntime(this, (_index) => { runtimeIndex = _index; });
        }
    }
    public void OnDestroy()
    {
        if (isInitFinish)
        {
            UnLoadStates();
            BTRuntimeController.RemoveRuntime(runtimeIndex);
        }

    }
}
public class TopologyData
{
    public string nodeId;
    public List<string> inDegree;
    public List<string> outDegree;
}
