using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;
using BTState = BehaviorTreeBaseState;

[Serializable]
public class SubBTContainerState : BTState
{
    #region AutoContext

    public System.Boolean exit;
    public System.Boolean enter;
    public BTTargetContainer container;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<SubBTContainerStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.exit = exit;
                _stateObj.enter = enter;
                _stateObj.container = container;
            }

            return _stateObj;
        }
    }
    private SubBTContainerStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);

        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(SubBTContainerStateObjJson));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            SubBTContainerStateObjJson _stateObjJson = (SubBTContainerStateObjJson)jsonSerializer.ReadObject(stream);
            _stateObj ??= new SubBTContainerStateObj();
            _stateObj.output = _stateObjJson.output;
            _stateObj.enter = _stateObjJson.enter;
            _stateObj.exit = _stateObjJson.exit;
            _stateObj.container.assetPath = _stateObjJson.container.assetPath;

            output = _stateObj.output;
            exit = _stateObj.exit;
            enter = _stateObj.enter;
            container = _stateObj.container;
        }
    }
    protected override ESetFieldValueResult SetFieldValue(string fieldName, object value)
    {
        if (StringComparer.Ordinal.Equals(fieldName, default)) return ESetFieldValueResult.Succ;

        else if (StringComparer.Ordinal.Equals(fieldName, "exit") && value is System.Boolean exitValue) exit = exitValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "enter") && value is System.Boolean enterValue) enter = enterValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "container") && value is BTTargetContainer containerValue) container = containerValue;
        else return ESetFieldValueResult.Fail;

        return ESetFieldValueResult.Succ;
    }
    public override void Save()
    {
        if (stateObj == null) return;
        output = _stateObj.output;
        interruptible = _stateObj.interruptible;
        interruptTag = _stateObj.interruptTag;

        exit = _stateObj.exit;
        enter = _stateObj.enter;
        container = _stateObj.container;
    }
    #endregion

    private BTRuntime subRuntime;
    public override void OnEnter()
    {
        base.OnEnter();

        bool isCanExecute = enter && runtime != null && !string.IsNullOrEmpty(container.assetPath);
        if (!string.IsNullOrEmpty(container.assetPath) && container.target == null)
        {
            OnRefresh();
            return;
        }

        if (isCanExecute) OnExecute();
        else OnExit();
    }
    public override void OnExecute()
    {
        LoadStates();
        base.OnExecute();
    }
    public override void OnUpdate()
    {
        base.OnUpdate();

        subRuntime.OnUpdate();
        if (subRuntime.isAllStateExecuteFinish) OnExit();
    }

    public override void OnExit()
    {
        if (subRuntime != null && subRuntime.isInitFinish)
        {
            subRuntime.UnLoadStates();
            BTRuntimeController.RemoveRuntime(subRuntime.runtimeIndex);
        }
        base.OnExit();
    }
    private void LoadStates()
    {
        subRuntime ??= new BTRuntime();
        subRuntime.gameObject = runtime.gameObject;
        subRuntime.transform = runtime.transform;
        subRuntime.container = container.target;
        SubRuntimeLoadState();
    }

    private void SubRuntimeLoadState()
    {
        if (!subRuntime.isInitFinish)
        {
            subRuntime.container = container.target;
            subRuntime.LoadStates();
            BTRuntimeController.AddRuntime(subRuntime, (_index) => { subRuntime.runtimeIndex = _index; });
        }
        else subRuntime.ResetAllStateAndRun();
    }
}

#region AutoContext_BTStateObject
public class SubBTContainerStateObj : BTStateObject
{
    public EBTState state;

    public bool exit;
    public bool enter;

    public BTTargetContainer container;

    public SubBTContainerStateObj()
    {
        if (container == null) container = new BTTargetContainer();
    }
}
#endregion
public class SubBTContainerStateObjJson : BTStateObject
{
    public EBTState state;

    public bool exit;
    public bool enter;

    public BTTargetContainerJson container;
}
public class BTTargetContainerJson
{
    public UnityEngine.Object target;
    public string assetPath;
}
