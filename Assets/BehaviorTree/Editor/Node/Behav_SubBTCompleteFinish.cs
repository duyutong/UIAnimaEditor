
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
public class Behav_SubBTCompleteFinish : BehaviorNode
{
    public override string stateName => "SubBTCompleteFinishState";
    public Behav_SubBTCompleteFinish() : base() 
    {
        title = "SubBTCompleteFinish";
        
        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(System.Boolean), Port.Capacity.Multi);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        
    }
}
