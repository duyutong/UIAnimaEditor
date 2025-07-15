
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class DragMoveState : BehaviorTreeBaseState
{
    #region AutoContext

    public Boolean exit;
    public Boolean enter;
    public BTTargetObject targetObj;
    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<DragMoveStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.exit = exit;
                _stateObj.enter = enter;
                _stateObj.targetObj = targetObj;
            }
            return _stateObj;
        }
    }
    private DragMoveStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(DragMoveStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (DragMoveStateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;

            exit = _stateObj.exit;
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

        exit = _stateObj.exit;
        enter = _stateObj.enter;
        targetObj = _stateObj.targetObj;
    }
    #endregion

    public PointerEventData pointerEventData;

    private RectTransform targetRect;
    private RectTransform parentRect;
    private Vector2 dragOffset;
    public override void OnEnter()
    {
        base.OnEnter();
        if (targetRect == null) targetRect = targetObj.target.GetComponent<RectTransform>();
        if (parentRect == null) parentRect = targetRect.parent.GetComponent<RectTransform>();

        bool isCanExecute = enter && runtime != null && pointerEventData != null;

        if (isCanExecute) OnExecute();
        else OnRefresh();
    }
    public override void OnExecute()
    {
        base.OnExecute();

        if (runtime == null) return;
        if (state != EBTState.执行中) return;

        OnBeginDrag();
        OnDrag();

        OnExit();
    }
    public override void OnRefresh()
    {
        base.OnRefresh();
        if (!enter) dragOffset = Vector2.zero;
    }
    private void OnDrag()
    {
        // 将屏幕坐标转换为 Canvas 空间坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, pointerEventData.position, pointerEventData.pressEventCamera, out Vector2 localPoint);
        // 设置 UI 元素的 anchoredPosition
        targetRect.anchoredPosition = localPoint + dragOffset;
    }
    private void OnBeginDrag()
    {
        if (dragOffset == Vector2.zero)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, pointerEventData.position, pointerEventData.pressEventCamera, out Vector2 localPoint);
            dragOffset = targetRect.anchoredPosition - localPoint;
        }
    }
}
public class DragMoveStateObj : BTStateObject
{
    public EBTState state;

    public Boolean exit;
    public Boolean enter;
    public BTTargetObject targetObj;
}
