
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;
[Serializable]
public class SnapArea2DState : BehaviorTreeBaseState
{
    #region AutoContext

    public System.Boolean isInSnapArea;
    public System.Boolean isOutOfSnapArea;
    public System.Boolean enter;
    public System.Single animaTime;
    public BTTargetObject areaObj;
    public BTTargetObject checkObj;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<SnapArea2DStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.isInSnapArea = isInSnapArea;
                _stateObj.isOutOfSnapArea = isOutOfSnapArea;
                _stateObj.enter = enter;
                _stateObj.animaTime = animaTime;
                _stateObj.areaObj = areaObj;
                _stateObj.checkObj = checkObj;
            }
            return _stateObj;
        }
    }
    private SnapArea2DStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(SnapArea2DStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (SnapArea2DStateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;

            isInSnapArea = _stateObj.isInSnapArea;
            isOutOfSnapArea = _stateObj.isOutOfSnapArea;
            enter = _stateObj.enter;
            animaTime = _stateObj.animaTime;
            areaObj = _stateObj.areaObj;
            checkObj = _stateObj.checkObj;
        }
    }
    protected override ESetFieldValueResult SetFieldValue(string fieldName, object value)
    {
        if (StringComparer.Ordinal.Equals(fieldName, default)) return ESetFieldValueResult.Succ;

        else if (StringComparer.Ordinal.Equals(fieldName, "isInSnapArea") && value is System.Boolean isInSnapAreaValue) isInSnapArea = isInSnapAreaValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "isOutOfSnapArea") && value is System.Boolean isOutOfSnapAreaValue) isOutOfSnapArea = isOutOfSnapAreaValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "enter") && value is System.Boolean enterValue) enter = enterValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "animaTime") && value is System.Single animaTimeValue) animaTime = animaTimeValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "areaObj") && value is BTTargetObject areaObjValue) areaObj = areaObjValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "checkObj") && value is BTTargetObject checkObjValue) checkObj = checkObjValue;
        else return ESetFieldValueResult.Fail;

        return ESetFieldValueResult.Succ;
    }
    public override void Save()
    {
        if (stateObj == null) return;
        output = _stateObj.output;
        interruptible = _stateObj.interruptible;
        interruptTag = _stateObj.interruptTag;

        isInSnapArea = _stateObj.isInSnapArea;
        isOutOfSnapArea = _stateObj.isOutOfSnapArea;
        enter = _stateObj.enter;
        animaTime = _stateObj.animaTime;
        areaObj = _stateObj.areaObj;
        checkObj = _stateObj.checkObj;
    }
    #endregion

    private RectTransform snapRect;
    private RectTransform checkRect;
    private Transform checkTrans;
    private Canvas canvas;

    private bool isOverlapping;

    public override void OnEnter()
    {
        base.OnEnter();

        isOverlapping = false;

        if (canvas == null) canvas = GameObject.FindObjectOfType<Canvas>();
        if (snapRect == null) snapRect = (areaObj.target as GameObject).GetComponent<RectTransform>();
        if (checkRect == null) checkRect = (checkObj.target as GameObject).GetComponent<RectTransform>();
        if (checkTrans == null) checkTrans = (checkObj.target as GameObject).transform;

        bool isCanExecute = enter && runtime != null && snapRect != null && checkRect != null;
        if (isCanExecute) CheckIsOverlapping();
        else OnRefresh();
    }
    private void CheckIsOverlapping()
    {
        Bounds boundsA = RectTransformUtility.CalculateRelativeRectTransformBounds(canvas.transform, snapRect);
        Bounds boundsB = RectTransformUtility.CalculateRelativeRectTransformBounds(canvas.transform, checkRect);

        isOverlapping = boundsA.Intersects(boundsB);

        isInSnapArea = isOverlapping;
        isOutOfSnapArea = !isOverlapping;

        for (int i = 0; i < output.Count; i++)
        {
            BTOutputInfo info = output[i];
            if (info.fromPortName == "isInSnapArea") info.value = isInSnapArea;
            if (info.fromPortName == "isOutOfSnapArea") info.value = isOutOfSnapArea;
        }

        if (isOverlapping) OnExecute();
        else OnExit();
    }
    public override void OnExecute()
    {
        base.OnExecute();
        StartSnap();
    }

    private Vector3 startPos;          // 初始位置
    private Vector3 targetPos;         // snapRect 的中心
    private float timer = 0f;          // 当前动画计时
    private bool isAnimating = false;  // 是否在动画中
    public override void OnUpdate()
    {
        base.OnUpdate();

        if (isAnimating)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / animaTime);

            // 插值移动
            checkTrans.position = Vector3.Lerp(startPos, targetPos, t);

            if (t >= 1f)
            {
                isAnimating = false; // 动画完成
                OnExit();
            }
        }
    }
    /// <summary>
    /// 开始执行移???
    /// </summary>
    private void StartSnap()
    {
        if (isAnimating) return;

        startPos = checkTrans.position;
        targetPos = GetRectWorldCenter(snapRect);
        timer = 0f;
        isAnimating = true;
    }

    /// <summary>
    /// 获取 RectTransform 的世界中心点
    /// </summary>
    private Vector3 GetRectWorldCenter(RectTransform rect)
    {
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);
        return (corners[0] + corners[2]) / 2f; // 左下角 + 右上角 / 2
    }
}

#region AutoContext_BTStateObject
public class SnapArea2DStateObj : BTStateObject
{
    public EBTState state;

    public System.Boolean isInSnapArea;
    public System.Boolean isOutOfSnapArea;
    public System.Boolean enter;
    public System.Single animaTime;
    public BTTargetObject areaObj;
    public BTTargetObject checkObj;
}
#endregion
