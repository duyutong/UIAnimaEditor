
using System;
using UnityEditor.Experimental.GraphView;
public class Trigger_SwitchButton : TriggerNode
{
    public override string stateName => "SwitchButtonState";
    public Trigger_SwitchButton() : base()
    {
        title = "SwitchButton";

        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(Boolean), Port.Capacity.Single);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        Port port_buttonObj = CreatePortForNode(this, Direction.Input, typeof(Boolean), Port.Capacity.Single);
        port_buttonObj.portName = "buttonObj";
        inputContainer.Add(port_buttonObj);

        Port port_switchOn = CreatePortForNode(this, Direction.Output, typeof(Boolean), Port.Capacity.Single);
        port_switchOn.portName = "switchOn";
        outputContainer.Add(port_switchOn);

        Port port_switchOff = CreatePortForNode(this, Direction.Output, typeof(Boolean), Port.Capacity.Single);
        port_switchOff.portName = "switchOff";
        outputContainer.Add(port_switchOff);

    }
}
