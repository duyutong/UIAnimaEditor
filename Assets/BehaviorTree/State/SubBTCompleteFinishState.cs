
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
[Serializable]
public class SubBTCompleteFinishState : BehaviorTreeBaseState
{
    #region AutoContext

    public System.Boolean enter;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<SubBTCompleteFinishStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.enter = enter;
            }
            return _stateObj;
        }
    }
    private SubBTCompleteFinishStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(SubBTCompleteFinishStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (SubBTCompleteFinishStateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;

            enter = _stateObj.enter;
        }
    }
    protected override ESetFieldValueResult SetFieldValue(string fieldName, object value)
    {
        if (StringComparer.Ordinal.Equals(fieldName, default)) return ESetFieldValueResult.Succ;

        else if (StringComparer.Ordinal.Equals(fieldName, "enter") && value is System.Boolean enterValue) enter = enterValue;
        else return ESetFieldValueResult.Fail;

        return ESetFieldValueResult.Succ;
    }
    public override void Save()
    {
        if (stateObj == null) return;
        output = _stateObj.output;
        interruptible = _stateObj.interruptible;
        interruptTag = _stateObj.interruptTag;

        enter = _stateObj.enter;
    }
    #endregion

    public override void OnEnter()
    {
        base.OnEnter();

        bool isCanExecute = enter && runtime != null;
        if (isCanExecute) OnExecute();
        else OnExit();
    }
    public override void OnExecute()
    {
        runtime.SetStateValue((_state) => { _state.state = EBTState.完成; }, (_state) => true);
        OnExit();
    }
}

#region AutoContext_BTStateObject
public class SubBTCompleteFinishStateObj : BTStateObject
{
    public EBTState state;

    public System.Boolean enter;
}
#endregion
