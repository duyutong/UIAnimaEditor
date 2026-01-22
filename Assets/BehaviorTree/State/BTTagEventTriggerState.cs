
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;
[Serializable]
public class BTTagEventTriggerState : TiggerBaseState
{
    #region AutoContext

    public Boolean exit;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<BTTagEventTriggerStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.exit = exit;
                _stateObj.triggerTag = triggerTag;
            }
            return _stateObj;
        }
    }
    private BTTagEventTriggerStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(BTTagEventTriggerStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (BTTagEventTriggerStateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;

            exit = _stateObj.exit;
            triggerTag = _stateObj.triggerTag;
        }
    }
    public override void Save()
    {
        if (stateObj == null) return;
        output = _stateObj.output;
        interruptible = _stateObj.interruptible;
        interruptTag = _stateObj.interruptTag;

        exit = _stateObj.exit;
        triggerTag = _stateObj.triggerTag;
    }
    #endregion
    public override void OnEnter()
    {
        base.OnEnter();

        exit = true;
        OnExit();
    }
    public override void OnExit()
    {
        for (int i = 0; i < output.Count; i++)
        {
            BTOutputInfo info = output[i];
            if (info.fromPortName == "exit") info.value = exit;
        }
        base.OnExit();
    }
}
public class BTTagEventTriggerStateObj : BTTiggerStateObject
{
    public EBTState state;
    public Boolean exit;
}
