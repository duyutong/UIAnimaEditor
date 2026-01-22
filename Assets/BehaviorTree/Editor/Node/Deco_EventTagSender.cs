
using System;
using UnityEditor.Experimental.GraphView;
public class Deco_EventTagSender : DecoratorNode
{
    public override string stateName => "EventTagSenderState";
    public Deco_EventTagSender() : base()
    {
        title = "EventTagSender";

        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(Boolean), Port.Capacity.Single);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        Port port_sendTag = CreatePortForNode(this, Direction.Input, typeof(String), Port.Capacity.Single);
        port_sendTag.portName = "sendTag";
        inputContainer.Add(port_sendTag);

        Port port_sendState = CreatePortForNode(this, Direction.Input, typeof(EBTState), Port.Capacity.Single);
        port_sendState.portName = "sendState";
        inputContainer.Add(port_sendState);


        Port port_exit = CreatePortForNode(this, Direction.Output, typeof(Boolean), Port.Capacity.Single);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);

    }
}
