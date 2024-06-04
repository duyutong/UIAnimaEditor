
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;
[Serializable]
public class ObjectNodeState : BehaviorTreeBaseState
{
    #region AutoContext

    public BTTargetObject obj;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<ObjectNodeStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.obj = obj;
            }
            return _stateObj;
        }
    }
    private ObjectNodeStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(ObjectNodeStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (ObjectNodeStateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;

            obj = _stateObj.obj;
        }
    }
    public override void Save()
    {
        if (stateObj == null) return;
        output = _stateObj.output;
        interruptible = _stateObj.interruptible;
        interruptTag = _stateObj.interruptTag;

        obj = _stateObj.obj;
    }
    #endregion
    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log(obj.target.name);
    }
}
public class ObjectNodeStateObj : BTStateObject
{
    public EBTState state;

    public BTTargetObject obj;
}
