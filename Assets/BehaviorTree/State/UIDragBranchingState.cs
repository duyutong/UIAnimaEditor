
using D.Unity3dTools;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class UIDragBranchingState : UIEventBranchingState
{
    #region AutoContext

    public System.Boolean beginDrag;
    public System.Boolean drag;
    public System.Boolean endDrag;
    public System.Boolean enter;
    public System.Boolean idel;
    public BTTargetObject targetObj;
    public BTTargetObject uiCameraObj;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<UIDragBranchingStateObj>();

                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.beginDrag = beginDrag;
                _stateObj.drag = drag;
                _stateObj.endDrag = endDrag;
                _stateObj.enter = enter;
                _stateObj.targetObj = targetObj;
                _stateObj.uiCameraObj = uiCameraObj;
                _stateObj.idel = idel;
            }

            return _stateObj;
        }
    }
    private UIDragBranchingStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(UIDragBranchingStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (UIDragBranchingStateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;

            beginDrag = _stateObj.beginDrag;
            drag = _stateObj.drag;
            endDrag = _stateObj.endDrag;
            enter = _stateObj.enter;
            targetObj = _stateObj.targetObj;
            uiCameraObj = _stateObj.uiCameraObj;
            idel = _stateObj.idel;
        }
    }
    public override void Save()
    {
        if (stateObj == null) return;
        output = _stateObj.output;
        interruptible = _stateObj.interruptible;
        interruptTag = _stateObj.interruptTag;

        beginDrag = _stateObj.beginDrag;
        drag = _stateObj.drag;
        endDrag = _stateObj.endDrag;
        enter = _stateObj.enter;
        targetObj = _stateObj.targetObj;
        uiCameraObj = _stateObj.uiCameraObj;
        idel = _stateObj.idel;
    }
    #endregion

    private RectTransform rectTransform;
    private Camera uiCamera;
    private bool isInitFinish;
    public override void OnEnter()
    {
        base.OnEnter();

        if (rectTransform == null && targetObj != null) rectTransform = targetObj.target.GetComponent<RectTransform>();
        if (uiCamera == null && uiCameraObj != null) uiCamera = uiCameraObj.target.GetComponent<Camera>();

        if (runtime != null && !isInitFinish)
        {
            isInitFinish = true;
            OnSetPointTriggerEvent(targetObj);

            trigger.AddTriggerEventListener(EventTriggerType.BeginDrag, OnMyBeginDrag);
            trigger.AddTriggerEventListener(EventTriggerType.Drag, OnMyDrag);
            trigger.AddTriggerEventListener(EventTriggerType.EndDrag, OnMyEndDrag);
            trigger.AddTriggerEventListener(EventTriggerType.PointerUp, OnPointerUp);
        }
        OnExecute();
    }
    public override void OnRefresh()
    {
        base.OnRefresh();

        if (drag) return;

        beginDrag = _stateObj.beginDrag;
        drag = _stateObj.drag;
        endDrag = _stateObj.endDrag;
        idel = _stateObj.idel;
    }
    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!isInitFinish) return;
        //Debug.Log($"beginDrag:{beginDrag}  drag:{drag}  endDrag:{endDrag}  idel:{idel}");

        OnExit();
    }
    public override void OnExit()
    {
        for (int i = 0; i < output.Count; i++)
        {
            BTOutputInfo info = output[i];
            if (info.fromPortName == "beginDrag") info.value = beginDrag;
            if (info.fromPortName == "drag") info.value = drag;
            if (info.fromPortName == "endDrag") info.value = endDrag;
            if (info.fromPortName == "idel") info.value = idel;
            output[i] = info;
        }

        base.OnExit();
    }
    private void OnPointerUp(PointerEventData data)
    {
        idel = true;
        drag = false;
        beginDrag = false;
        endDrag = false;
    }
    private void OnMyDrag(PointerEventData data)
    {
        drag = true;
        beginDrag = false;
        endDrag = false;
        idel = false;
    }

    private void OnMyEndDrag(PointerEventData data)
    {
        drag = false;
        beginDrag = false;
        endDrag = true;
        idel = false;
    }

    private void OnMyBeginDrag(PointerEventData data)
    {
        drag = false;
        beginDrag = true;
        endDrag = false;
        idel = false;
    }
}
public class UIDragBranchingStateObj : BTStateObject
{
    public EBTState state;

    public System.Boolean beginDrag;
    public System.Boolean drag;
    public System.Boolean endDrag;
    public System.Boolean enter;
    public System.Boolean idel;
    public BTTargetObject targetObj;
    public BTTargetObject uiCameraObj;
}
