
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;
[Serializable]
public class InvokeActionState : BehaviorTreeBaseState
{
    #region AutoContext

    public System.Boolean exit;
    public BTTargetEvent btEvent;
    public System.Boolean enter;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<InvokeActionStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.exit = exit;
                _stateObj.btEvent = btEvent;
                _stateObj.enter = enter;
            }
            return _stateObj;
        }
    }
    private InvokeActionStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(InvokeActionStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (InvokeActionStateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;

            exit = _stateObj.exit;
            btEvent = _stateObj.btEvent;
            enter = _stateObj.enter;
        }
    }
    public override void Save()
    {
        if (stateObj == null) return;
        output = _stateObj.output;
        interruptible = _stateObj.interruptible;
        interruptTag = _stateObj.interruptTag;

        exit = _stateObj.exit;
        btEvent = _stateObj.btEvent;
        enter = _stateObj.enter;
    }
    #endregion
    public override void OnEnter()
    {
        base.OnEnter();

        bool isCanExecute = enter && runtime != null;
        if (isCanExecute) OnExecute();
        else OnRefresh();
    }
    public override void OnExecute()
    {
        base.OnExecute();
        btEvent.targetEvent?.Invoke();
        OnExit();
    }
}
public class InvokeActionStateObj : BTStateObject
{
    public EBTState state;

    public System.Boolean exit;
    public BTTargetEvent btEvent;
    public System.Boolean enter;
}
