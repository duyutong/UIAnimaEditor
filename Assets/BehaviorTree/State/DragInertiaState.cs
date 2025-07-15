
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class DragInertiaState : BehaviorTreeBaseState
{
    #region AutoContext

    public Boolean exit;
    public Boolean enter;
    public Single speed;
    public Single AxisOffset_Z;
    public BTTargetObject targetObj;
    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<DragInertiaStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.exit = exit;
                _stateObj.enter = enter;
                _stateObj.speed = speed;
                _stateObj.AxisOffset_Z = AxisOffset_Z;
                _stateObj.targetObj = targetObj;
            }
            return _stateObj;
        }
    }
    private DragInertiaStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(DragInertiaStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (DragInertiaStateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;

            exit = _stateObj.exit;
            enter = _stateObj.enter;
            speed = _stateObj.speed;
            AxisOffset_Z = _stateObj.AxisOffset_Z;
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
        speed = _stateObj.speed;
        AxisOffset_Z = _stateObj.AxisOffset_Z;
        targetObj = _stateObj.targetObj;
    }
    #endregion

    public PointerEventData pointerEventData;

    private Vector2 lastPos;

    private RectTransform parentRect;
    private RectTransform targetRect;
    private RectTransform canvasRect;
    private Canvas canvas;
    public override void OnCheck()
    {
        base.OnCheck();

        if (canvas == null) canvas = GameObject.FindObjectOfType<Canvas>();
        if (targetRect == null) targetRect = targetObj.target.GetComponent<RectTransform>();
        if (parentRect == null) parentRect = targetRect.parent.GetComponent<RectTransform>();
        if (canvasRect == null) canvasRect = canvas.GetComponent<RectTransform>();

        if (state != EBTState.执行中) RefreshRotation();
    }
    public override void OnEnter()
    {
        base.OnEnter();

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
    public override void OnExit()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, pointerEventData.position, pointerEventData.pressEventCamera, out Vector2 localPoint);
        lastPos = localPoint;
        base.OnExit();
    }
    private void OnDrag()
    {
        // 将屏幕坐标转换为 Canvas 空间坐标
        RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRect, pointerEventData.position, pointerEventData.pressEventCamera, out Vector2 posInSelf);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, pointerEventData.position, pointerEventData.pressEventCamera, out Vector2 localPoint);
        Vector2 delta = localPoint - lastPos;
        //设置惯性效果
        Vector3 currAngles = ResetRotation();
        int direction = posInSelf.y > 0 ? -1 : 1;
        float targetZ = Mathf.Lerp(currAngles.z, direction * delta.x, speed * Time.deltaTime);
        targetZ = Math.Clamp(targetZ, -1 * AxisOffset_Z, AxisOffset_Z);
        targetRect.localEulerAngles = targetZ * Vector3.forward;
    }
    private void OnBeginDrag()
    {
        if (lastPos != Vector2.zero) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, pointerEventData.position, pointerEventData.pressEventCamera, out Vector2 localPoint);
        lastPos = localPoint;
    }
    public void RefreshRotation()
    {
        if (!enter)
        {
            Vector3 currAngles = ResetRotation();
            float targetZ = Mathf.Lerp(currAngles.z, 0, speed * Time.deltaTime);
            float deltaZ = targetZ - currAngles.z;

            Vector3 deltaVec3 = deltaZ * Vector3.forward;
            targetRect.localEulerAngles += deltaVec3;
        }
    }
    private Vector3 ResetRotation()
    {
        Vector3 currAngles = targetRect.localEulerAngles;
        while (currAngles.z > AxisOffset_Z) currAngles.z -= 360;
        while (currAngles.z < -AxisOffset_Z) currAngles.z += 360;
        targetRect.localEulerAngles = currAngles;
        return currAngles;
    }
}
public class DragInertiaStateObj : BTStateObject
{
    public EBTState state;

    public Boolean exit;
    public Boolean enter;
    public Single speed;
    public Single AxisOffset_Z;
    public BTTargetObject targetObj;
}
