
using D.Unity3dTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;
[Serializable]
public class EventTagSenderState : BehaviorTreeBaseState
{
    #region AutoContext

    public Boolean exit;
    public Boolean enter;
    public String sendTag;
    public EBTState sendState;
    public EBTTagSendType tagSendType;
    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<EventTagSenderStateObj>();

                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.exit = exit;
                _stateObj.enter = enter;
                _stateObj.sendTag = sendTag;
                _stateObj.sendState = sendState;
                _stateObj.tagSendType = tagSendType;
            }
            return _stateObj;
        }
    }
    private EventTagSenderStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(EventTagSenderStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (EventTagSenderStateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;

            exit = _stateObj.exit;
            enter = _stateObj.enter;
            sendTag = _stateObj.sendTag;
            sendState = _stateObj.sendState;
            tagSendType = _stateObj.tagSendType;
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
        sendTag = _stateObj.sendTag;
        sendState = _stateObj.sendState;
        tagSendType = _stateObj.tagSendType;
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
        base.OnExecute();
        if (runtime != null)
        {
            if (tagSendType == EBTTagSendType.Local) runtime.OnReceiveMsg(sendTag, sendState);
            if (tagSendType == EBTTagSendType.Scene) BTRuntimeController.ins.SendToTag(sendTag, sendState);
        }
        OnExit();
    }
}
public class EventTagSenderStateObj : BTStateObject
{
    public EBTState state;

    public Boolean exit;
    public Boolean enter;

    public EBTTagSendType tagSendType;
    public EBTState sendState;
    public String sendTag;
}
public enum EBTTagSendType
{
    Local,
    Scene,
}
