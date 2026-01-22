using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine.UIElements;

public class ProtSettingView : VisualElement
{
    private BTNodePortSetting portSetting;
    public Action<ProtSettingView> onDelPort;
    public Action onAddPort;

    private List<VisualElement> customVEList = new List<VisualElement>();
    private Button subBtn;
    private Button addBtn;
    private Button delBtn;
    private Toggle showInNodeToggle;

    private bool isShowAdd = false;
    private bool isShowSub = false;
    private bool isShowDel = false;

    private EnumField directionField;
    private EnumField capacityField;
    public new class UxmlFactory : UxmlFactory<ProtSettingView, UxmlTraits> { }
    public void ShowProtSetting(BTNodePortSetting _portSetting, bool showAdd = false)
    {
        Type structType = typeof(BTNodePortSetting);
        FieldInfo[] fields = structType.GetFields();
        portSetting = _portSetting;

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
                element.value = field.GetValue(_portSetting) as string;
                element.name = fieldName;
                element.RegisterValueChangedCallback((_str) =>
                {
                    field.SetValue(portSetting, _str.newValue);
                });
                Add(element);
                customVEList.Add(element);
            }
            if (fieldType.IsEnum)
            {
                object fieldValue = field.GetValue(portSetting);
                EnumField element = new EnumField(fieldName, fieldValue as Enum);
                element.value = fieldValue as Enum;
                element.name = fieldName;
                if (fieldName == "direction") directionField = element;
                if (fieldName == "capacity") capacityField = element;
                element.RegisterValueChangedCallback((_enum) =>
                {
                    field.SetValue(portSetting, _enum.newValue);
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

        showInNodeToggle = new Toggle("isShowAsPort");
        showInNodeToggle.style.position = Position.Absolute;
        showInNodeToggle.style.right = new Length(0, LengthUnit.Percent);
        showInNodeToggle.SetValueWithoutNotify(portSetting.isShowAsPort);
        showInNodeToggle.Q<Label>().style.fontSize = 10;
        showInNodeToggle.RegisterValueChangedCallback(v => portSetting.isShowAsPort = v.newValue);
        buttonGroup.Add(showInNodeToggle);

        Add(buttonGroup);
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

        portSetting.node.AddPortForNode(portSetting);
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

        portSetting.node.RemovePortFromNode(portSetting.portName, portSetting.direction);
        foreach (VisualElement element in customVEList) element.SetEnabled(true);
    }
    private void OnClickDelete()
    {
        onDelPort?.Invoke(this);
    }
}
