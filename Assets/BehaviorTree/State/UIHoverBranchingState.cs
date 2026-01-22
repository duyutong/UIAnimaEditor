
using D.Unity3dTools;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class UIHoverBranchingState : UIEventBranchingState
{
    #region AutoContext

    public System.Boolean pointerEnter;
    public System.Boolean hover;
    public System.Boolean pointerExit;
    public System.Boolean idel;
    public System.Boolean enter;
    public BTTargetObject targetObj;
    public BTTargetObject uiCameraObj;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<UIHoverBranchingStateObj>();

                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.pointerEnter = pointerEnter;
                _stateObj.hover = hover;
                _stateObj.pointerExit = pointerExit;
                _stateObj.idel = idel;
                _stateObj.enter = enter;
                _stateObj.targetObj = targetObj;
                _stateObj.uiCameraObj = uiCameraObj;
            }

            return _stateObj;
        }
    }
    private UIHoverBranchingStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(UIHoverBranchingStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (UIHoverBranchingStateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;

            pointerEnter = _stateObj.pointerEnter;
            hover = _stateObj.hover;
            pointerExit = _stateObj.pointerExit;
            idel = _stateObj.idel;
            enter = _stateObj.enter;
            targetObj = _stateObj.targetObj;
            uiCameraObj = _stateObj.uiCameraObj;
        }
    }
    public override void Save()
    {
        if (stateObj == null) return;
        output = _stateObj.output;
        interruptible = _stateObj.interruptible;
        interruptTag = _stateObj.interruptTag;

        pointerEnter = _stateObj.pointerEnter;
        hover = _stateObj.hover;
        pointerExit = _stateObj.pointerExit;
        idel = _stateObj.idel;
        enter = _stateObj.enter;
        targetObj = _stateObj.targetObj;
        uiCameraObj = _stateObj.uiCameraObj;
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

            trigger.AddTriggerEventListener(EventTriggerType.PointerEnter, OnMyPointerEnter);
            trigger.AddTriggerEventListener(EventTriggerType.PointerExit, OnMyPointerExit);
            trigger.AddTriggerEventListener(EventTriggerType.PointerDown, OnMyPointerDown);
            trigger.AddTriggerEventListener(EventTriggerType.PointerUp, OnMyPointerEnter);//鼠标弹起的时候默认再次进入范围
        }

        bool isOver = IsMouseOverUIElement(rectTransform, uiCamera);
        if (isOver && pointerEnter)
        {
            pointerExit = false;
            pointerEnter = false;
            hover = true;
            idel = false;
            OnExit();
        }

        if (hover) OnExit();
        if (idel) OnExit();
    }
    public override void OnRefresh()
    {
        base.OnRefresh();
        bool isOver = IsMouseOverUIElement(rectTransform, uiCamera);
        if (!isOver && pointerExit)
        {
            pointerExit = false;
            pointerEnter = false;
            hover = false;
            idel = true;
        }
    }
    public override void OnExit()
    {
        for (int i = 0; i < output.Count; i++)
        {
            BTOutputInfo info = output[i];
            if (info.fromPortName == "pointerExit") info.value = pointerExit;
            if (info.fromPortName == "pointerEnter") info.value = pointerEnter;
            if (info.fromPortName == "hover") info.value = hover;
            if (info.fromPortName == "idel") info.value = idel;
        }
        base.OnExit();
    }
    private void OnMyPointerDown(PointerEventData data)
    {
        pointerExit = false;
        pointerEnter = false;
        hover = false;
        idel = true;

        OnExit();
    }

    private void OnMyPointerExit(PointerEventData data)
    {
        pointerExit = true;
        pointerEnter = false;
        hover = false;
        idel = false;

        OnExit();
    }

    private void OnMyPointerEnter(PointerEventData data)
    {
        pointerEnter = true;
        pointerExit = false;
        hover = false;
        idel = false;

        OnExit();
    }
}
public class UIHoverBranchingStateObj : BTStateObject
{
    public EBTState state;

    public System.Boolean pointerEnter;
    public System.Boolean hover;
    public System.Boolean pointerExit;
    public System.Boolean idel;
    public System.Boolean enter;
    public BTTargetObject targetObj;
    public BTTargetObject uiCameraObj;
}
