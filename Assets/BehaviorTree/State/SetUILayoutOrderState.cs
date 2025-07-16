
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SetUILayoutOrderState : BehaviorTreeBaseState
{
    #region AutoContext

    public System.Boolean exit;
    public System.Boolean enter;
    public BTTargetObject targetObj;
    public System.Int32 sortingOrder;
    public System.String sortingLayerName;

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null)
            {
                _stateObj = ScriptableObject.CreateInstance<SetUILayoutOrderStateStateObj>();
                _stateObj.state = state;
                _stateObj.output = output;
                _stateObj.interruptible = interruptible;
                _stateObj.interruptTag = interruptTag;

                _stateObj.exit = exit;
                _stateObj.enter = enter;
                _stateObj.targetObj = targetObj;
                _stateObj.sortingOrder = sortingOrder;
                _stateObj.sortingLayerName = sortingLayerName;
            }
            return _stateObj;
        }
    }
    private SetUILayoutOrderStateStateObj _stateObj;
    public override void InitParam(string param)
    {
        base.InitParam(param);
        DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(SetUILayoutOrderStateStateObj));
        using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(param)))
        {
            _stateObj = (SetUILayoutOrderStateStateObj)jsonSerializer.ReadObject(stream);
            output = _stateObj.output;

            exit = _stateObj.exit;
            enter = _stateObj.enter;
            targetObj = _stateObj.targetObj;
            sortingOrder = _stateObj.sortingOrder;
            sortingLayerName = _stateObj.sortingLayerName;
        }
    }
    protected override ESetFieldValueResult SetFieldValue(string fieldName, object value)
    {
        if (StringComparer.Ordinal.Equals(fieldName, default)) return ESetFieldValueResult.Succ;

        else if (StringComparer.Ordinal.Equals(fieldName, "exit") && value is System.Boolean exitValue) exit = exitValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "enter") && value is System.Boolean enterValue) enter = enterValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "targetObj") && value is BTTargetObject targetObjValue) targetObj = targetObjValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "sortingOrder") && value is System.Int32 sortingOrderValue) sortingOrder = sortingOrderValue;
        else if (StringComparer.Ordinal.Equals(fieldName, "sortingLayerName") && value is System.String sortingLayerNameValue) sortingLayerName = sortingLayerNameValue;
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
        targetObj = _stateObj.targetObj;
        sortingOrder = _stateObj.sortingOrder;
        sortingLayerName = _stateObj.sortingLayerName;
    }
    #endregion

    private Canvas canvas;
    public override void OnEnter()
    {
        base.OnEnter();

        if (canvas == null)
        {
            targetObj.target.GetOrAddComponent<Canvas>();
            targetObj.target.GetOrAddComponent<GraphicRaycaster>();

            canvas = targetObj.target.GetComponent<Canvas>();
            canvas.overrideSorting = true;
        }

        bool isCanExecute = enter && runtime != null;
        if (isCanExecute) OnExecute();
        else OnRefresh();
    }
    public override void OnExecute()
    {
        base.OnExecute();
        if (canvas == null) return;

       
        canvas.sortingLayerName = sortingLayerName;
        canvas.sortingOrder = sortingOrder;

        OnExit();
    }
}

#region AutoContext_BTStateObject
public class SetUILayoutOrderStateStateObj : BTStateObject
{
    public EBTState state;

    public System.Boolean exit;
    public System.Boolean enter;
    public BTTargetObject targetObj;
    public System.Int32 sortingOrder;
    public System.String sortingLayerName;
}
#endregion
