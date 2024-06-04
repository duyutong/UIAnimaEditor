
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
public class Behav_SetUILayoutOrder : BehaviorNode
{
    public override string stateName => "SetUILayoutOrderState";
    public Behav_SetUILayoutOrder() : base() 
    {
        title = "SetUILayoutOrder";
        
        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(Boolean), Port.Capacity.Single);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        Port port_order = CreatePortForNode(this, Direction.Input, typeof(Int32), Port.Capacity.Single);
        port_order.portName = "order";
        inputContainer.Add(port_order);

        Port port_target = CreatePortForNode(this, Direction.Input, typeof(BTTargetObject), Port.Capacity.Single);
        port_target.portName = "target";
        inputContainer.Add(port_target);

        Port port_exit = CreatePortForNode(this, Direction.Output, typeof(Boolean), Port.Capacity.Multi);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);

    }
}
