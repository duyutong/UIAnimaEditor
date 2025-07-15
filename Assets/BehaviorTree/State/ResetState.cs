
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
[Serializable]
public class ResetState : BehaviorTreeBaseState
{
    #region AutoContext

    public System.Boolean enter;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<ResetStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.enter = enter;
            }
            return _stateObj;
        }
    }
    private ResetStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(ResetStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (ResetStateObj)jsonSerializer.ReadObject(stream);
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
        else OnRefresh();
    }
    public override void OnExecute()
    {
        base.OnExecute();
        Infect((_s) =>
        {
            _s.OnRefresh();
        }, (_s) =>
        {
            return false;
        });
    }
}

#region AutoContext_BTStateObject
public class ResetStateObj : BTStateObject
{
    public EBTState state;

    public System.Boolean enter;
}
#endregion
