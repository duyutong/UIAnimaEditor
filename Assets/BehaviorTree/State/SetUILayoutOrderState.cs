
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SetUILayoutOrderState : BehaviorTreeBaseState
{
    #region AutoContext

    public Boolean exit;
    public Boolean enter;
    public Int32 order;
    public BTTargetObject target;
    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<SetUILayoutOrderStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.exit = exit;
                _stateObj.enter = enter;
                _stateObj.order = order;
                _stateObj.target = target;
            }
            return _stateObj;
        }
    }
    private SetUILayoutOrderStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(SetUILayoutOrderStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (SetUILayoutOrderStateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;

            exit = _stateObj.exit;
            enter = _stateObj.enter;
            order = _stateObj.order;
            target = _stateObj.target;
        }
    }
    public override void Save()
    {
        if (stateObj == null) return;
        output = _stateObj.output;
        interruptible = _stateObj.interruptible;
        interruptTag = _stateObj.interruptTag;

        exit = _stateObj.exit;
        enter = _stateObj.enter;
        order = _stateObj.order;
        target = _stateObj.target;
    }
    #endregion

    private Canvas canvas;
    public override void OnEnter()
    {
        base.OnEnter();
        if (runtime != null && canvas == null) 
        {
            canvas = target.target.GetOrAddComponent<Canvas>();
            runtime.GetOrAddComponent<GraphicRaycaster>();
        }

        bool isCanExecute = enter && runtime != null;
        if (isCanExecute) OnExecute();
        else OnExit();
    }
    public override void OnExecute()
    {
        base.OnExecute();
        if (canvas != null) 
        {
            canvas.overrideSorting = true;
            canvas.sortingOrder = order;
        }

        OnExit();
    }
}
public class SetUILayoutOrderStateObj : BTStateObject
{
    public EBTState state;

    public Boolean exit;
    public Boolean enter;
    public Int32 order;
    public BTTargetObject target;

    public List<BTOutputInfo> output;
}
