
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
public class Behav_ScaleAnima : BehaviorNode
{
    public override string stateName => "ScaleAnimaState";
    public Behav_ScaleAnima() : base() 
    {
        title = "ScaleAnima";

        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(Boolean), Port.Capacity.Single);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        Port port_animaCurve = CreatePortForNode(this, Direction.Input, typeof(AnimationCurve), Port.Capacity.Single);
        port_animaCurve.portName = "animaCurve";
        inputContainer.Add(port_animaCurve);

        Port port_target = CreatePortForNode(this, Direction.Input, typeof(BTTargetObject), Port.Capacity.Single);
        port_target.portName = "target";
        inputContainer.Add(port_target);

        Port port_exit = CreatePortForNode(this, Direction.Output, typeof(Boolean), Port.Capacity.Single);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);

    }
}
