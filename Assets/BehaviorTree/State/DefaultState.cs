using System;

[Serializable]
public class DefaultState : BehaviorTreeBaseState
{
    public new string stateName => "DefaultState";

    public override BTStateObject stateObj
    {
        get
        {
            if (_stateObj == null) _stateObj = new DefaultStateObj();
            return _stateObj;
        }
    }
    private DefaultStateObj _stateObj;
    public override void OnEnter() { }

    public override void OnExecute() { }

    public override void OnExit() { }

    public override void OnUpdate() { }
}
public class DefaultStateObj : BTStateObject
{
    public string text = "DefaultState";
}
