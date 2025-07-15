﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class LoopState : BehaviorTreeBaseState
{
    public ELoopState loopState = ELoopState.循环开始;
    public string loopTag;
    public int loopCount = 0;

    private LoopState lastLoopStartState;
    private int currCount;
    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<LoopStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.loopState = loopState;
                _stateObj.loopCount = loopCount;
                _stateObj.state = state;
                _stateObj.loopTag = loopTag;
            }
            return _stateObj;
        }
    }
    private LoopStateObj _stateObj;

    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(LoopStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (LoopStateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;

            loopState = _stateObj.loopState;
            loopCount = _stateObj.loopCount;
            currCount = _stateObj.loopCount;
            loopTag = _stateObj.loopTag;
        }
    }
    public override void OnInitFinish()
    {
        FindLastLoopStart();
    }
    public override void Save()
    {
        if (stateObj == null) return;
        output = _stateObj.output;
        interruptible = _stateObj.interruptible;
        interruptTag = _stateObj.interruptTag;

        loopState = _stateObj.loopState;
        loopCount = _stateObj.loopCount;
        loopTag = _stateObj.loopTag;
    }
    public override void OnEnter()
    {
        if (loopState == ELoopState.循环开始) OnExit();
        else
        {
            if (loopCount >= 0)
            {
                --currCount;
                if (currCount <= 0) { OnExit(); return; }
            }

            OnExecute();
        }
    }
    public override void OnExecute() 
    {
        base.OnExecute();

        Infect((_s) =>
        {
            _s.OnRefresh();
        }, (_s) =>
        {
            if (_s.stateName != "LoopState") return false;
            if ((_s as LoopState).loopState != ELoopState.循环开始) return false;
            return true;

        });

        lastLoopStartState.OnEnter();
    }
    private void FindLastLoopStart()
    {
        lastLoopStartState = GetLastMatchingState<LoopState>((_s) =>
        {
            if (_s.loopState != ELoopState.循环开始) return false;
            if (_s.loopTag != loopTag) return false;
            return true;
        });
    }
}

public class LoopStateObj : BTStateObject
{
    public EBTState state;
    public ELoopState loopState;
    public string loopTag;
    public int loopCount;
}

[Serializable]
public enum ELoopState
{
    循环开始,
    循环结束,
}
