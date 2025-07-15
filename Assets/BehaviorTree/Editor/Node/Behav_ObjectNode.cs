
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
public class Behav_ObjectNode : BehaviorNode
{
    public override string stateName => "ObjectNodeState";
    public Behav_ObjectNode() : base() 
    {
        title = "ObjectNode";
        
        Port port_obj = CreatePortForNode(this, Direction.Input, typeof(BTTargetObject), Port.Capacity.Single);
        port_obj.portName = "obj";
        inputContainer.Add(port_obj);
    }
}
