
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
public class Behav_SubBTContainer : BehaviorNode
{
    public override string stateName => "SubBTContainerState";
    public Behav_SubBTContainer() : base() 
    {
        title = "SubBTContainer";
        
        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(System.Boolean), Port.Capacity.Multi);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        Port port_container = CreatePortForNode(this, Direction.Input, typeof(BTTargetContainer), Port.Capacity.Single);
        port_container.portName = "container";
        inputContainer.Add(port_container);

        
        Port port_exit = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);

    }
}
