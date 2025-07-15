
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SwitchButtonState : BehaviorTreeBaseState
{
    #region AutoContext

    public Boolean switchOn;
    public Boolean switchOff;
    public Boolean enter;
    public BTTargetObject buttonObj;
    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<SwitchButtonStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.switchOn = switchOn;
                _stateObj.switchOff = switchOff;
                _stateObj.buttonObj = buttonObj;
                _stateObj.enter = enter;
            }
            return _stateObj;
        }
    }
    private SwitchButtonStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(SwitchButtonStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (SwitchButtonStateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;

            switchOn = _stateObj.switchOn;
            switchOff = _stateObj.switchOff;
            enter = _stateObj.enter;
            buttonObj = _stateObj.buttonObj;

            currSwitchState = switchOn;//手动初始化状态值
        }
    }
    public override void Save()
    {
        if (stateObj == null) return;
        output = _stateObj.output;
        interruptible = _stateObj.interruptible;
        interruptTag = _stateObj.interruptTag;

        switchOn = _stateObj.switchOn;
        switchOff = _stateObj.switchOff;
        buttonObj = _stateObj.buttonObj;
        enter = _stateObj.enter;
    }
    #endregion

    private Button button;
    private bool currSwitchState;
    public override void OnEnter()
    {
        base.OnEnter();
        if (button == null) 
        {
            button = buttonObj.target.GetComponent<Button>();
            button.onClick.AddListener(() => OnExecute());
        }
    }
    public override void OnExecute()
    {
        base.OnExecute();

        currSwitchState = !currSwitchState;
        switchOn = currSwitchState;
        switchOff = !currSwitchState;

        OnExit();
    }
    public override void OnExit()
    {
        for (int i = 0; i < output.Count; i++) 
        {
            BTOutputInfo info = output[i];

            if (info.fromPortName == "switchOn") info.value = switchOn;
            if (info.fromPortName == "switchOff") info.value = switchOff;
        }

        switchOn = false;
        switchOff = false;
        base.OnExit();
    }
}
public class SwitchButtonStateObj : BTStateObject
{
    public EBTState state;

    public Boolean switchOn;
    public Boolean switchOff;
    public BTTargetObject buttonObj;
    public Boolean enter;
}
