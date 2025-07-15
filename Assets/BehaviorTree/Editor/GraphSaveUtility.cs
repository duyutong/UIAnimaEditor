﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using BTBaseNode = BehaviorTreeBaseNode;
using Unity.Plastic.Newtonsoft.Json;
using Edge = UnityEditor.Experimental.GraphView.Edge;

public static class GraphSaveUtility
{
    private static List<BTTargetObject> bTTargetObjects = new List<BTTargetObject>();
    private static List<BTTargetEvent> bTTargetEvents = new List<BTTargetEvent>();
    private static List<BTTargetContainer> bTTargetContainers = new List<BTTargetContainer>();
    /// <summary>
    /// 保存节点和连线数据到ScriptableObject，并将其存储在指定文件路径
    /// </summary>
    /// <param name="fileName">要保存的文件名</param>
    /// <param name="nodes">要保存的节点</param>
    /// <param name="edges">要保存的连线</param>
    public static void SaveData(string fileName, UQueryState<Node> nodes, UQueryState<Edge> edges)
    {
        string filePath = $"Assets/BehaviorTree/BT/{fileName}.asset";
        bool isExist = File.Exists(filePath) ;
        BTContainer container;
        if (isExist) container = AssetDatabase.LoadAssetAtPath<BTContainer>(filePath) ;
        else container = ScriptableObject.CreateInstance<BTContainer>();

        container.edgeDatas.Clear();
        container.nodeDatas.Clear();
        bTTargetObjects.Clear();

        foreach (Edge edge in edges)
        {
            Node outNode = edge.output.node;
            BTBaseNode outBaseNode = outNode as BTBaseNode;
            if (outBaseNode == null) continue;

            Node inputNode = edge.input.node;
            BTBaseNode inputBaseNode = inputNode as BTBaseNode;
            if (inputBaseNode == null) continue;

            EdgeData data = new EdgeData();
            data.outPortNode = outBaseNode.guid;
            data.outPortName = edge.output.portName;
            data.intputPortNode = inputBaseNode.guid;
            data.intputPortName = edge.input.portName;
            container.edgeDatas.Add(data);
        }
        foreach (Node node in nodes)
        {
            BTBaseNode baseNode = node as BTBaseNode;
            if (baseNode == null) continue;
            baseNode.btState.Save();
            NodeData data = new NodeData();
            Dictionary<string, object> _variableValues = new Dictionary<string, object>();
            FieldInfo[] fields = baseNode.btState.GetType().GetFields();
            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(baseNode.btState);
                _variableValues.Add(field.Name, value);
            }
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(baseNode.btState.stateObj.GetType());
            string json = "";
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                CheckAndProcessObjectFields(baseNode.btState.stateObj);
                jsonSerializer.WriteObject(stream, baseNode.btState.stateObj);
                json = Encoding.UTF8.GetString(stream.ToArray());
            }
           
            data.lastNodes = new List<string>();
            foreach (BTBaseNode _node in baseNode.lastNodes) data.lastNodes.Add(_node.guid);
            data.nodeName = baseNode.title;
            data.stateParams = json;
            data.guid = baseNode.guid;
            data.nodePos = baseNode.nodePos;
            data.typeName = baseNode.GetType().FullName;
            data.output = baseNode.btState.output;
            data.stateName = baseNode.stateName;
            container.nodeDatas.Add(data);
        }

        if (isExist) EditorUtility.SetDirty(container);
        else AssetDatabase.CreateAsset(container, filePath);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        ResetBTTarget();
        Debug.Log($"{fileName}保存完成");
    }
    private static void ResetBTTarget() 
    {
        foreach (BTTargetObject bTTargetObject in bTTargetObjects) 
        {
            if (bTTargetObject == null) continue;
            bTTargetObject.SetObejctByPath();
        }
        bTTargetObjects.Clear();

        foreach (BTTargetEvent bTTargetEvent in bTTargetEvents)
        {
            if (bTTargetEvent == null) continue;
            bTTargetEvent.SetTargetEvent();
        }
        bTTargetEvents.Clear();

        foreach (BTTargetContainer bTTargetContainer in bTTargetContainers) 
        {
            if (bTTargetContainer == null) continue;
            bTTargetContainer.SetContainer();
        }
        bTTargetContainers.Clear();
    }
    private static void CheckAndProcessObjectFields(BTStateObject bTState) 
    {
        Type type = bTState.GetType();
        if (type == null) return;
        FieldInfo[] fields = type.GetFields(BindingFlags.Public| BindingFlags.Instance);
        foreach (FieldInfo field in fields) 
        {
            if (field.FieldType == typeof(BTTargetObject)) 
            {
                BTTargetObject bTTargetObject = (BTTargetObject)field.GetValue(bTState);
                if (bTTargetObject != null) bTTargetObject.SerializeSelf();
                bTTargetObjects.Add(bTTargetObject);
            }
            if (field.FieldType == typeof(BTTargetEvent)) 
            {
                BTTargetEvent bTTargetEvent = (BTTargetEvent)field.GetValue(bTState);
                if (bTTargetEvent != null) bTTargetEvent.SerializeSelf();
                bTTargetEvents.Add(bTTargetEvent);
            }
            if(field.FieldType == typeof(BTTargetContainer))
            {
                BTTargetContainer bTTargetContainer = (BTTargetContainer)field.GetValue(bTState);
                if(bTTargetContainer != null)bTTargetContainer.SerializeSelf();
                bTTargetContainers.Add(bTTargetContainer);
            }
        }
    }
    
    #region 自定义节点生成
    private static bool CheckIsTrigger(DefaultNode node) 
    {
        return node.nodeType == "TriggerNode";
    }
    private static string GetStateExtends(DefaultNode node) 
    {
        if (node.nodeType == "TriggerNode") return "TiggerBaseState";
        return "BehaviorTreeBaseState";
    }
    private static string GetStateObject(DefaultNode node)
    {
        if (node.nodeType == "TriggerNode") return "BTTiggerStateObject";
        return "BTStateObject";
    }
    /// <summary>
    /// 生成节点对应状态的C#脚本
    /// </summary>
    /// <param name="node">节点</param>
    public static void GenStateToCSharp(DefaultNode node)
    {
        string tempStr = CSTemplate.stateStr;
        string stateExtends = GetStateExtends(node);
        string stateObject = GetStateObject(node);

        string stateName = node.title;
        string className = $"{stateName}State";

        tempStr = tempStr.Replace("#StateName#", stateName);
        tempStr = tempStr.Replace("#BaseState#", stateExtends);
        tempStr = tempStr.Replace("#BTStateObject#", stateObject);

        string str1 = "";
        string str2 = "";
        string str3 = "";
        string str4 = "";
        List<Port> oPorts = node.outputContainer.Query<Port>().ToList();
        List<Port> iPorts = node.inputContainer.Query<Port>().ToList();
        List<Port> ports = new List<Port>();
        ports.AddRange(oPorts);
        ports.AddRange(iPorts);
        foreach (Port port in ports)
        {
            string oStrTemp1 = CSTemplate.initPropStr1;
            oStrTemp1 = oStrTemp1.Replace("#PortName#", port.portName);
            oStrTemp1 = oStrTemp1.Replace("#PortType#", port.portType.FullName);
            str1 += oStrTemp1;

            string oStrTemp2 = CSTemplate.initPropStr2;
            oStrTemp2 = oStrTemp2.Replace("#PortName#", port.portName);
            str2 += oStrTemp2;

            string oStrTemp3 = CSTemplate.initPropStr3;
            oStrTemp3 = oStrTemp3.Replace("#PortName#", port.portName);
            str3 += oStrTemp3;

            string oStrTemp4 = CSTemplate.initPropStr4;
            oStrTemp4 = oStrTemp4.Replace("#PortName#", port.portName);
            oStrTemp4 = oStrTemp4.Replace("#PortType#", port.portType.FullName);
            str4 += oStrTemp4;
        }

        bool isTrigger = CheckIsTrigger(node);
        if (isTrigger) str2 += CSTemplate.initPropStr2_trigger;

        tempStr = tempStr.Replace("#PublicProperty#", str1);
        tempStr = tempStr.Replace("#SetPropValue#", str2);
        tempStr = tempStr.Replace("#SetObjPropValue#", str3);
        tempStr = tempStr.Replace("#SetFieldValue#",str4);

        //写入文件
        string csSavePath = Application.dataPath.Replace("\\", "/") + "/BehaviorTree/State/" + className + ".cs";
        FileInfo saveInfo = new FileInfo(csSavePath);
        DirectoryInfo dir = saveInfo.Directory;
        if (!dir.Exists) dir.Create();
        byte[] decBytes = Encoding.UTF8.GetBytes(tempStr);

        FileStream fileStream = saveInfo.Create();
        fileStream.Write(decBytes, 0, decBytes.Length);
        fileStream.Flush();
        fileStream.Close();

        AssetDatabase.Refresh();
        Debug.Log("状态脚本生成完毕 " + className);
    }

    /// <summary>
    /// 生成节点对应的C#脚本
    /// </summary>
    /// <param name="node">要生成C#脚本的节点</param>
    public static void GenNodeToCSharp(DefaultNode node)
    {
        string nodeType = node.nodeType;
        BTBaseNode baseNode = Activator.CreateInstance(Type.GetType(nodeType)) as BTBaseNode;
        string tempStr = CSTemplate.nodeStr;
        string prefix = baseNode.Prefix;
        string title = node.title;

        string className = $"{prefix}_{title}";

        tempStr = tempStr.Replace("#Prefix#", prefix);
        tempStr = tempStr.Replace("#NodeType#", nodeType);
        tempStr = tempStr.Replace("#Title#", title);

        string iStr = "";
        List<Port> iPorts = node.inputContainer.Query<Port>().ToList();
        foreach (Port port in iPorts)
        {
            if (port.style.display == DisplayStyle.None) continue;

            string iStrTemp = CSTemplate.inputContainerStr;
            iStrTemp = iStrTemp.Replace("#PortName#", port.portName);
            iStrTemp = iStrTemp.Replace("#PortType#", port.portType.FullName);
            iStrTemp = iStrTemp.Replace("#Capacity#", Enum.GetName(typeof(Port.Capacity), port.capacity));
            iStr += iStrTemp;
        }

        string oStr = "";
        List<Port> oPorts = node.outputContainer.Query<Port>().ToList();
        foreach (Port port in oPorts)
        {
            string oStrTemp = CSTemplate.outputContainerStr;
            oStrTemp = oStrTemp.Replace("#PortName#", port.portName);
            oStrTemp = oStrTemp.Replace("#PortType#", port.portType.FullName);
            oStrTemp = oStrTemp.Replace("#Direction#", Enum.GetName(typeof(Direction), port.direction));
            oStrTemp = oStrTemp.Replace("#Capacity#", Enum.GetName(typeof(Port.Capacity), port.capacity));
            oStr += oStrTemp;
        }

        tempStr = tempStr.Replace("#InputContainer#", iStr);
        tempStr = tempStr.Replace("#OutputContainer#", oStr);

        //写入文件
        string csSavePath = Application.dataPath.Replace("\\", "/") + "/BehaviorTree/Editor/Node/" + className + ".cs";
        FileInfo saveInfo = new FileInfo(csSavePath);
        DirectoryInfo dir = saveInfo.Directory;
        if (!dir.Exists) dir.Create();
        byte[] decBytes = Encoding.UTF8.GetBytes(tempStr);

        FileStream fileStream = saveInfo.Create();
        fileStream.Write(decBytes, 0, decBytes.Length);
        fileStream.Flush();
        fileStream.Close();

        AssetDatabase.Refresh();
        Debug.Log("节点脚本生成完毕 " + className);
    }
    #endregion

    #region 复制粘贴功能

    /// <summary>
    /// 将图形元素序列化为JSON字符串
    /// </summary>
    /// <param name="elements">要序列化的图形元素</param>
    /// <returns>序列化后的JSON字符串</returns>
    public static string SerializeGraphElements(IEnumerable<GraphElement> elements)
    {
        BTContainer_Copy container = new BTContainer_Copy();

        List<Node> nodes = new List<Node>();
        List<Edge> edges = new List<Edge>();

        List<string> existentNodes = new List<string>();
        foreach (GraphElement element in elements)
        {
            Node node = element as Node;
            Edge edge = element as Edge;
            if (node != null) nodes.Add(node);
            if (edge != null) edges.Add(edge);
        }

        foreach (Node node in nodes)
        {     
            BTBaseNode baseNode = node as BTBaseNode;
            if (baseNode == null) continue;
            baseNode.btState.Save();
            NodeData data = new NodeData();
            Dictionary<string, object> _variableValues = new Dictionary<string, object>();
            FieldInfo[] fields = baseNode.btState.GetType().GetFields();
            foreach (FieldInfo field in fields)
            {
                object value = field.GetValue(baseNode.btState);
                _variableValues.Add(field.Name, value);
            }
            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(baseNode.btState.stateObj.GetType());
            string json = "";
            using (MemoryStream stream = new System.IO.MemoryStream())
            {
                jsonSerializer.WriteObject(stream, baseNode.btState.stateObj);
                json = Encoding.UTF8.GetString(stream.ToArray());
            }
            data.lastNodes = new List<string>();
            foreach (BTBaseNode _node in baseNode.lastNodes) data.lastNodes.Add(_node.guid);
            data.nodeName = baseNode.title;
            data.stateParams = json;
            data.guid = baseNode.guid;
            data.nodePos = baseNode.GetPosition().position;
            data.typeName = baseNode.GetType().FullName;
            data.output = baseNode.btState.output;
            data.stateName = baseNode.stateName;

            existentNodes.Add(baseNode.guid);
            container.nodeDatas.Add(data);
        }
        foreach (Edge edge in edges)
        {
            Node outNode = edge.output.node;
            BTBaseNode outBaseNode = outNode as BTBaseNode;
            if (outBaseNode == null) continue;
            if (!existentNodes.Contains(outBaseNode.guid)) continue;

            Node inputNode = edge.input.node;
            BTBaseNode inputBaseNode = inputNode as BTBaseNode;
            if (inputBaseNode == null) continue;
            if (!existentNodes.Contains(inputBaseNode.guid)) continue;

            EdgeData data = new EdgeData();
            data.outPortNode = outBaseNode.guid;
            data.outPortName = edge.output.portName;
            data.intputPortNode = inputBaseNode.guid;
            data.intputPortName = edge.input.portName;
            container.edgeDatas.Add(data);
        }
        return SerializeObject(container);
    }

    /// <summary>
    /// 将对象序列化为 JSON 字符串
    /// </summary>
    /// <param name="obj">要序列化的对象</param>
    /// <returns>序列化后的JSON字符串</returns>
    public static string SerializeObject(object obj)
    {
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(obj.GetType());
        string json = "";
        using (MemoryStream stream = new MemoryStream())
        {
            jsonSerializer.WriteObject(stream, obj);
            json = Encoding.UTF8.GetString(stream.ToArray());
        }
        return json;
    }

    /// <summary>
    /// 将 JSON 字符串反序列化为对象
    /// </summary>
    /// <typeparam name="T">要反序列化的对象类型</typeparam>
    /// <param name="json">要反序列化的JSON字符串</param>
    /// <returns>反序列化后的对象</returns>
    public static T DeserializeObject<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }

    #endregion
}

[Serializable]
public class CustomNodeData
{
    public NodeData nodeData;
    public List<BTNodePortSetting> portSettings;
}
