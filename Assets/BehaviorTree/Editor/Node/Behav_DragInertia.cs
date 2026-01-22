
using System;
using UnityEditor.Experimental.GraphView;
public class Behav_DragInertia : BehaviorNode
{
    public override string stateName => "DragInertiaState";
    public Behav_DragInertia() : base()
    {
        title = "DragInertia";

        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(Boolean), Port.Capacity.Single);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        Port port_speed = CreatePortForNode(this, Direction.Input, typeof(Single), Port.Capacity.Single);
        port_speed.portName = "speed";
        inputContainer.Add(port_speed);

        Port port_AxisOffset_Z = CreatePortForNode(this, Direction.Input, typeof(Single), Port.Capacity.Single);
        port_AxisOffset_Z.portName = "AxisOffset_Z";
        inputContainer.Add(port_AxisOffset_Z);

        Port port_targetObj = CreatePortForNode(this, Direction.Input, typeof(BTTargetObject), Port.Capacity.Single);
        port_targetObj.portName = "targetObj";
        inputContainer.Add(port_targetObj);

        Port port_exit = CreatePortForNode(this, Direction.Output, typeof(Boolean), Port.Capacity.Single);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);

    }
}
