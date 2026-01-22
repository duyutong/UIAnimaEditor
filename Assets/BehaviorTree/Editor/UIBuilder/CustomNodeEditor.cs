using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomNodeEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset visualTreeAsset = default;

    //NodeView
    private CustomNodeView nodeView;

    //ToolBar
    private TextField nameTextField;
    private DropdownField nodeTypeField;
    private Button createBtn;
    private List<string> nodeTypeList = new List<string>();

    //PortSetting
    private VisualElement settingView;
    private ScrollView scrollView;
    private Button addPortBtn;
    private Button savePortBtn;
    private DefaultNode currNode;

    [MenuItem("BehaviourTreeEditor/Open NodeEditor")]
    public static void OpenWindow()
    {
        CustomNodeEditor wnd = GetWindow<CustomNodeEditor>();
        wnd.titleContent = new GUIContent("CustomNodeEditor");
    }

    public void CreateGUI()
    {
        VisualElement root = rootVisualElement;
        visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/BehaviorTree/Editor/UIBuilder/CustomNodeEditor.uxml");
        visualTreeAsset.CloneTree(root);

        InitNodeTypeList();

        nodeView = root.Q<CustomNodeView>();
        nodeView.onSelectAction = OnSelectAction;
        nodeView.onUnselectAction = OnUnselectAction;

        nameTextField = root.Q<TextField>("nameTextField");
        nameTextField.RegisterValueChangedCallback((_value) =>
        {
            if (currNode != null)
            {
                currNode.title = _value.newValue;
                currNode.RefreshExpandedState();
            }
        });

        nodeTypeField = root.Q<DropdownField>("nodeTypeField");
        nodeTypeField.choices = nodeTypeList;
        nodeTypeField.value = nodeTypeList[0];
        nodeTypeField.RegisterValueChangedCallback((_value) =>
        {
            if (currNode != null)
            {
                currNode.nodeType = _value.newValue;
                currNode.RefreshExpandedState();
            }
        });

        createBtn = root.Q<Button>("createBtn");
        createBtn.clicked += OnClickCreateBtn;

        addPortBtn = root.Q<Button>("add");
        addPortBtn.clicked += OnClickAddPortBtn;

        savePortBtn = root.Q<Button>("save");
        savePortBtn.clicked += OnClickSaveBtn;

        scrollView = root.Q<ScrollView>("scrollView");
        settingView = root.Q<VisualElement>("settingView");
        settingView.style.display = DisplayStyle.None;
    }
    private void InitNodeTypeList()
    {
        nodeTypeList.Clear();
        string path = Application.dataPath.Replace("\\", "/") + "/BehaviorTree/Editor/Node/Base";
        DirectoryInfo directory = Directory.CreateDirectory(path);
        FileInfo[] fileInfos = directory.GetFiles();
        foreach (FileInfo info in fileInfos)
        {
            if (info.Extension.ToLower() != ".cs") continue;
            nodeTypeList.Add(info.Name.Split(".")[0]);
        }
    }
    private void OnClickSaveBtn()
    {
        GraphSaveUtility.GenNodeToCSharp(currNode);
        GraphSaveUtility.GenStateToCSharp(currNode);
    }
    private void OnClickAddPortBtn()
    {
        ProtSettingView settingView = new ProtSettingView();
        BTNodePortSetting portSetting = new BTNodePortSetting();
        portSetting.node = currNode;
        settingView.ShowProtSetting(portSetting, true);
        settingView.onDelPort = OnDeletePort;
        scrollView.Add(settingView);
    }

    private void OnClickCreateBtn()
    {
        string nodeType = nodeTypeField.value;
        string nodeName = nameTextField.text;
        if (string.IsNullOrEmpty(nodeName)) return;
        nodeView.CreatNode(nodeName, nodeType);
    }
    private void OnSelectAction(BehaviorTreeBaseNode _node)
    {
        DefaultNode node = _node as DefaultNode;
        currNode = node;
        if (node == null) return;

        scrollView.contentContainer.Clear();
        //遍历node的所有接口
        List<Port> ports = node.inputContainer.Query<Port>().ToList();
        ports.AddRange(node.outputContainer.Query<Port>().ToList());

        foreach (Port port in ports)
        {
            ProtSettingView settingView = new ProtSettingView();
            BTNodePortSetting portSetting = new BTNodePortSetting();
            portSetting.node = _node;
            portSetting.portName = port.portName;
            portSetting.direction = port.direction;
            portSetting.capacity = port.capacity;
            portSetting.portType = (EPortType)Enum.Parse(typeof(EPortType), port.portType.Name);

            settingView.ShowProtSetting(portSetting);
            settingView.onDelPort = OnDeletePort;
            scrollView.Add(settingView);
        }

        settingView.style.display = DisplayStyle.Flex;
    }
    private void OnDeletePort(ProtSettingView view)
    {
        scrollView.contentContainer.Remove(view);
    }
    private void OnUnselectAction()
    {
        currNode = null;
        settingView.style.display = DisplayStyle.None;
    }
}