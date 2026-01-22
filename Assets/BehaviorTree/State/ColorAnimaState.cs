using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ColorAnimaState : BehaviorTreeBaseState
{
    #region AutoContext
    public Boolean exit;
    public Boolean enter;
    public Color color;
    public BTTargetObject target;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<ColorAnimaStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.exit = exit;
                _stateObj.enter = enter;
                _stateObj.color = color;
                _stateObj.target = target;
            }
            return _stateObj;
        }
    }
    private ColorAnimaStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(ColorAnimaStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (ColorAnimaStateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;

            exit = _stateObj.exit;
            enter = _stateObj.enter;
            color = _stateObj.color;
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
        color = _stateObj.color;
        target = _stateObj.target;
    }
    #endregion

    private RectTransform targetRect;
    private Graphic graphic;
    private float crossTime = 0.1f;
    private float timeCount;
    public override void OnEnter()
    {
        base.OnEnter();
        if (targetRect == null) targetRect = target.target.GetComponent<RectTransform>();
        if (graphic == null) graphic = targetRect.GetComponent<Graphic>();

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
        if (targetRect == null) return;
        if (state != EBTState.执行中) return;

        timeCount += Time.deltaTime;
        if (timeCount >= crossTime)
        {
            timeCount = 0;
            OnExit();
        }
        else
        {
            Color currColor = graphic.color;
            graphic.color = Color.Lerp(currColor, color, timeCount);
        }
    }
}
public class ColorAnimaStateObj : BTStateObject
{
    public EBTState state;

    public Boolean exit;
    public Boolean enter;
    public Color color;
    public BTTargetObject target;
}
