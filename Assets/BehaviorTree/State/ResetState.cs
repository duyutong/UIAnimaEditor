
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;
[Serializable]
public class ResetState : BehaviorTreeBaseState
{
    #region AutoContext

    public Boolean exit;

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

                _stateObj.exit = exit;

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

            exit = _stateObj.exit;
        }
    }
    public override void Save()
    {
        if (stateObj == null) return;
        output = _stateObj.output;
        interruptible = _stateObj.interruptible;
        interruptTag = _stateObj.interruptTag;

        exit = _stateObj.exit;
    }
    #endregion
    public override void OnEnter()
    {
        base.OnEnter();
        OnExecute();
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
public class ResetStateObj : BTStateObject
{
    public EBTState state;

    public Boolean exit;
}
