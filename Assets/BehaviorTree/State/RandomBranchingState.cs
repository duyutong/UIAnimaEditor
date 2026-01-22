using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using UnityEngine;
[Serializable]
public class RandomBranchingState : BehaviorTreeBaseState
{
    #region AutoContext

    public System.Boolean exit;
    public System.Boolean enter;
    public System.Int32 parallelCount;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<RandomBranchingStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.exit = exit;
                _stateObj.enter = enter;
                _stateObj.parallelCount = parallelCount;
            }
            return _stateObj;
        }
    }
    private RandomBranchingStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(RandomBranchingStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (RandomBranchingStateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;

            exit = _stateObj.exit;
            enter = _stateObj.enter;
            parallelCount = _stateObj.parallelCount;
        }
    }
    protected override ESetFieldValueResult SetFieldValue(string fieldName, object value)
    {
        if (StringComparer.Ordinal.Equals(fieldName, default)) return ESetFieldValueResult.Succ;

        else if (StringComparer.Ordinal.Equals(fieldName, "exit") && value is System.Boolean exitValue) exit = exitValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "enter") && value is System.Boolean enterValue) enter = enterValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "parallelCount") && value is System.Int32 parallelCountValue) parallelCount = parallelCountValue;
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
        parallelCount = _stateObj.parallelCount;
    }
    #endregion
    private List<BehaviorTreeBaseState> nextStates;
    public override void OnInitFinish()
    {
        if (nextStates == null) nextStates = GetNextStates();
    }
    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("--- RandomBranchingState OnEnter ---");
        if (nextStates.Count > 0)
        {
            List<int> randomNumbers = GenerateRandomNumbers(0, nextStates.Count, parallelCount);
            for (int i = 0; i < nextStates.Count; i++)
            {
                int _index = i;
                bool isSelect = randomNumbers.Contains(_index);

                if (!isSelect) nextStates[_index].state = EBTState.空;
                else nextStates[_index].OnRefresh();
            }
        }

        OnExit();
    }
    private List<int> GenerateRandomNumbers(int minValue, int maxValue, int n)
    {
        if (n > maxValue - minValue + 1 || n < 0)
        {
            throw new ArgumentException("Invalid input parameters. Unable to generate unique random numbers.");
        }

        List<int> randomNumbers = new List<int>();
        System.Random random = new System.Random();

        while (randomNumbers.Count < n)
        {
            int randomNumber = random.Next(minValue, maxValue);
            if (!randomNumbers.Contains(randomNumber))
            {
                randomNumbers.Add(randomNumber);
            }
        }

        return randomNumbers;
    }
    public override void OnExit()
    {
        for (int i = 0; i < output.Count; i++) 
        {
            BTOutputInfo info = output[i];
            if (info.fromPortName == "exit") info.value = true;
        }

        base.OnExit();
    }
}

#region AutoContext_BTStateObject
public class RandomBranchingStateObj : BTStateObject
{
    public EBTState state;

    public System.Boolean exit;
    public System.Boolean enter;
    public System.Int32 parallelCount;
}
#endregion
