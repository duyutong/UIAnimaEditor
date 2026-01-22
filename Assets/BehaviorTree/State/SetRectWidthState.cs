
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
[Serializable]
public class SetRectWidthState : BehaviorTreeBaseState
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
                _stateObj = ScriptableObject.CreateInstance<SetRectWidthStateObj>();

                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.exit = exit;
                _stateObj.enter = enter;
                _stateObj.animaCurve = animaCurve;
                _stateObj.target = target;
            }

            return _stateObj;
        }
    }
    private SetRectWidthStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(SetRectWidthStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (SetRectWidthStateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;

            exit = _stateObj.exit;
            enter = _stateObj.enter;
            animaCurve = _stateObj.animaCurve;
            target = _stateObj.target;
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
            float width = animaCurve.Evaluate(timeCount);
            targetRect.sizeDelta = new Vector2(width, targetRect.rect.height);
        }
    }
}
public class SetRectWidthStateObj : BTStateObject
{
    public EBTState state;

    public Boolean exit;
    public Boolean enter;
    public AnimationCurve animaCurve;
    public BTTargetObject target;
}
