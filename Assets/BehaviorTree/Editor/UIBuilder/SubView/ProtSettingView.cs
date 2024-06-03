using Codice.CM.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.GraphView.Port;

public class ProtSettingView : VisualElement
{
    private BTNodePortSetting sPortInfo;
    public Action<ProtSettingView> onDelPort;
    public Action onAddPort;

    private List<VisualElement> customVEList = new List<VisualElement>();
    private Button subBtn;
    private Button addBtn;
    private Button delBtn;

    private bool isShowAdd = false;
    private bool isShowSub = false;
    private bool isShowDel = false;

    private EnumField directionField;
    private EnumField capacityField;
    public new class UxmlFactory : UxmlFactory<ProtSettingView, UxmlTraits> { }
    public void ShowProtSetting(BTNodePortSetting info, bool showAdd = false)
    {
        Type structType = typeof(BTNodePortSetting);
        FieldInfo[] fields = structType.GetFields();
        sPortInfo = info;

        isShowAdd = showAdd;
        isShowDel = isShowAdd;
        isShowSub = !isShowDel;

        foreach (FieldInfo field in fields)
        {
            string fieldName = field.Name;
            Type fieldType = field.FieldType;

            if (fieldType.IsEquivalentTo(typeof(string)))
            {
                TextField element = new TextField(fieldName);
                element.value = field.GetValue(info) as string;
                element.name = fieldName;
                element.RegisterValueChangedCallback((_str) =>
                {
                    field.SetValue(sPortInfo, _str.newValue);
                });
                Add(element);
                customVEList.Add(element);
            }
            if (fieldType.IsEnum)
            {
                object fieldValue = field.GetValue(sPortInfo);
                EnumField element = new EnumField(fieldName, fieldValue as Enum);
                element.value = fieldValue as Enum;
                element.name = fieldName;
                if (fieldName == "direction") directionField = element;
                if (fieldName == "capacity") capacityField = element;
                element.RegisterValueChangedCallback((_enum) =>
                {
                    //ToTreeStructure(_enum.newValue);
                    field.SetValue(sPortInfo, _enum.newValue);
                });
                Add(element);
                customVEList.Add(element);
            }
        }

        VisualElement buttonGroup = new VisualElement();
        buttonGroup.style.flexDirection = FlexDirection.Row;
        buttonGroup.style.height = 20;

        foreach (VisualElement element in customVEList) element.SetEnabled(showAdd);

        addBtn = new Button(OnClickAddBtn);
        addBtn.text = "+";
        buttonGroup.Add(addBtn);

        subBtn = new Button(OnClickSubBtn);
        subBtn.text = "-";
        buttonGroup.Add(subBtn);

        delBtn = new Button(OnClickDelete);
        delBtn.text = "×";
        buttonGroup.Add(delBtn);

        addBtn.style.display = isShowAdd ? DisplayStyle.Flex : DisplayStyle.None;
        subBtn.style.display = isShowSub ? DisplayStyle.Flex : DisplayStyle.None;
        delBtn.style.display = isShowDel ? DisplayStyle.Flex : DisplayStyle.None;

        Add(buttonGroup);
    }
    /// <summary>
    /// 通过规定输入口和输出口的capacity值，使节点编辑结果呈树状结构
    /// 不使用该函数不影响其他功能
    /// 只是目前发现节点存储顺序对运行结果有影响，所以强制树形结构，方便排序
    /// *不对，还是觉得不应该是存储顺序的问题，因为无论存储顺序是怎样的，运行顺序都是一样的
    /// </summary>
    private void ToTreeStructure(Enum newValue)
    {
        if (capacityField == null) return;
        if (newValue.Equals(Direction.Input))
        {
            capacityField.SetValueWithoutNotify(Capacity.Single);
            capacityField.SetEnabled(false);
        }
        else if (newValue.Equals(Direction.Output))
        {
            capacityField.SetEnabled(true);
        }
        else if (newValue.Equals(Capacity.Multi)) 
        {
            bool isInput = directionField.value.Equals(Direction.Input);
            if (isInput) ToTreeStructure(Direction.Input);
            Debug.Log("树形结构：当接口为输入端时，只能接收单程输入");
        }
    }
    private bool CheckInput()
    {
        bool result = true;
        //检查是否有字符类型的参数没填
        foreach (VisualElement element in customVEList)
        {
            TextField textField = element as TextField;
            if (textField != null && string.IsNullOrEmpty(textField.text))
            {
                result = false;
                break;
            }
        }
        return result;
    }
    private void OnClickAddBtn()
    {
        bool checkInput = CheckInput();
        if (!checkInput)
        {
            string title = "InputError";
            string message = "Warning: The input parameters are incorrect. Please review the ProtSetting panel for accuracy and make the necessary corrections.";
            string okButton = "OK";

            EditorUtility.DisplayDialog(title, message, okButton);
            return;
        }

        isShowAdd = false;
        isShowSub = true;
        isShowDel = false;

        addBtn.style.display = isShowAdd ? DisplayStyle.Flex : DisplayStyle.None;
        subBtn.style.display = isShowSub ? DisplayStyle.Flex : DisplayStyle.None;
        delBtn.style.display = isShowDel ? DisplayStyle.Flex : DisplayStyle.None;

        sPortInfo.node.AddPortForNode(sPortInfo);
        foreach (VisualElement element in customVEList) element.SetEnabled(false);
        onAddPort?.Invoke();
    }
    private void OnClickSubBtn()
    {
        isShowAdd = true;
        isShowSub = false;
        isShowDel = true;

        addBtn.style.display = isShowAdd ? DisplayStyle.Flex : DisplayStyle.None;
        subBtn.style.display = isShowSub ? DisplayStyle.Flex : DisplayStyle.None;
        delBtn.style.display = isShowDel ? DisplayStyle.Flex : DisplayStyle.None;

        sPortInfo.node.RemovePortFromNode(sPortInfo.portName, sPortInfo.direction);
        foreach (VisualElement element in customVEList) element.SetEnabled(true);
    }
    private void OnClickDelete()
    {
        onDelPort?.Invoke(this);
    }
}
