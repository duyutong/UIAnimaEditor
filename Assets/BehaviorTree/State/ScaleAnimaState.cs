
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
[Serializable]
public class ScaleAnimaState : BehaviorTreeBaseState
{
    #region AutoContext
    public Boolean exit;
    public Boolean enter;
    public AnimationCurve animaCurve;
    public BTTargetObject target;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<ScaleAnimaStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.exit = exit;
                _stateObj.enter = enter;
                _stateObj.animaCurve = animaCurve;
                _stateObj.target = target;
            }

            _stateObj.output = output;
            return _stateObj;
        }
    }
    private ScaleAnimaStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(ScaleAnimaStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (ScaleAnimaStateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;

            exit = _stateObj.exit;
            enter = _stateObj.enter;
            animaCurve = _stateObj.animaCurve;
            target = _stateObj.target;
        }
    }
    protected override ESetFieldValueResult SetFieldValue(string fieldName, object value)
    {
        if (StringComparer.Ordinal.Equals(fieldName, "exit") && value is bool exitValue) exit = exitValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "enter") && value is bool enterValue) enter = enterValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "animaCurve") && value is AnimationCurve curveValue) animaCurve = curveValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "target") && value is BTTargetObject targetValue) target = targetValue;
        else return ESetFieldValueResult.Fail;

        return ESetFieldValueResult.Succ;
    }
    public override void Save()
    {
        if (stateObj == null) return;
        output = _stateObj.output;
        interruptible = _stateObj.interruptible;
        interruptTag = _stateObj.interruptTag;

        exit = _stateObj.exit;
        enter = _stateObj.enter;
        animaCurve = _stateObj.animaCurve;
        target = _stateObj.target;
    }
    #endregion

    private float startTime;
    private float endTime;
    private float timeCount;
    private RectTransform targetRect;
    public override void OnEnter()
    {
        base.OnEnter();
        if (targetRect == null) targetRect = target.target.GetComponent<RectTransform>();
        (startTime, endTime) = GetCurveTimeRange(animaCurve);

        bool isCanExecute = enter && runtime != null;
        if (isCanExecute) OnExecute();
        else OnRefresh();
    }
    public override void OnRefresh()
    {
        base.OnRefresh();
        timeCount = 0;
    }
    public override void OnUpdate()
    {
        if (runtime == null) return;
        if (state != EBTState.执行中) return;

        timeCount += Time.deltaTime;
        if (timeCount > endTime)
        {
            OnExit();
            return;
        }

        if (startTime <= timeCount && timeCount <= endTime)
        {
            float scaleRatio = animaCurve.Evaluate(timeCount);
            targetRect.localScale = scaleRatio * Vector3.one;
        }
    }
}
public class ScaleAnimaStateObj : BTStateObject
{
    public EBTState state;
    public Boolean exit;
    public Boolean enter;
    public AnimationCurve animaCurve;
    public BTTargetObject target;
}
