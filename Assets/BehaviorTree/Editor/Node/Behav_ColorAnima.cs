
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public class Behav_ColorAnima : BehaviorNode
{
    public override string stateName => "ColorAnimaState";
    public Behav_ColorAnima() : base()
    {
        title = "ColorAnima";

        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(Boolean), Port.Capacity.Single);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        Port port_color = CreatePortForNode(this, Direction.Input, typeof(Color), Port.Capacity.Single);
        port_color.portName = "color";
        inputContainer.Add(port_color);

        Port port_target = CreatePortForNode(this, Direction.Input, typeof(BTTargetObject), Port.Capacity.Single);
        port_target.portName = "target";
        inputContainer.Add(port_target);


        Port port_exit = CreatePortForNode(this, Direction.Output, typeof(Boolean), Port.Capacity.Single);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);

    }
}
