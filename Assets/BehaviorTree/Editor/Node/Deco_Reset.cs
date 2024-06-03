
using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
public class Deco_Reset : DecoratorNode
{
    public override string stateName => "ResetState";
    public Deco_Reset() : base() 
    {
        title = "Reset";
        
        Port port_exit = CreatePortForNode(this, Direction.Input, typeof(Boolean), Port.Capacity.Single);
        port_exit.portName = "exit";
        inputContainer.Add(port_exit);

        
    }
}
