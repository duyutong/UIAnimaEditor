
using D.Unity3dTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class UIDownOrUpBranchingState : UIEventBranchingState
{
    #region AutoContext

    public System.Boolean pointerDown;
    public System.Boolean pointerUp;
    public System.Boolean idel;
    public System.Boolean enter;
    public BTTargetObject targetObj;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<UIDownOrUpBranchingStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.pointerDown = pointerDown;
                _stateObj.pointerUp = pointerUp;
                _stateObj.idel = idel;
                _stateObj.enter = enter;
                _stateObj.targetObj = targetObj;
            }
            return _stateObj;
        }
    }
    private UIDownOrUpBranchingStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(UIDownOrUpBranchingStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (UIDownOrUpBranchingStateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;

            pointerDown = _stateObj.pointerDown;
            pointerUp = _stateObj.pointerUp;
            idel = _stateObj.idel;
            enter = _stateObj.enter;
            targetObj = _stateObj.targetObj;
        }
    }
    public override void Save()
    {
        if (stateObj == null) return;
        output = _stateObj.output;
        interruptible = _stateObj.interruptible;
        interruptTag = _stateObj.interruptTag;

        pointerDown = _stateObj.pointerDown;
        pointerUp = _stateObj.pointerUp;
        idel = _stateObj.idel;
        enter = _stateObj.enter;
        targetObj = _stateObj.targetObj;
    }
    #endregion

    private RectTransform rectTransform;
    private bool isInitFinish;

    public override void OnEnter() 
    {
        base.OnEnter();
        if (rectTransform == null && targetObj != null) rectTransform = targetObj.target.GetComponent<RectTransform>();
        if (runtime != null && !isInitFinish) 
        {
            isInitFinish = true;
            OnSetPointTriggerEvent(targetObj);
            trigger.AddTriggerEventListener(EventTriggerType.PointerDown, OnMyPointerDown);
            trigger.AddTriggerEventListener(EventTriggerType.PointerUp, OnMyPointerUp);
        }
    }
    public override void OnRefresh() 
    {
        base.OnRefresh();

        pointerDown = _stateObj.pointerDown;
        pointerUp = _stateObj.pointerUp;
        idel = _stateObj.idel;
    }

    public override void OnExit()
    {
        for (int i = 0; i < output.Count; i++)
        {
            BTOutputInfo info = output[i];
            if (info.fromPortName == "pointerUp") info.value = pointerUp;
            if (info.fromPortName == "pointerDown") info.value = pointerDown;
            if (info.fromPortName == "idel") info.value = idel;
        }
        base.OnExit();
    }
    private void OnMyPointerUp(PointerEventData data)
    {
        pointerUp = true; 
        pointerDown = false;
        idel = false;

        OnExit();
    }

    private void OnMyPointerDown(PointerEventData data)
    {
        pointerUp = false;
        pointerDown = true;
        idel = false;

        OnExit();
    }
}
public class UIDownOrUpBranchingStateObj : BTStateObject
{
    public EBTState state;

    public System.Boolean pointerDown;
    public System.Boolean pointerUp;
    public System.Boolean idel;
    public System.Boolean enter;
    public BTTargetObject targetObj;
}
