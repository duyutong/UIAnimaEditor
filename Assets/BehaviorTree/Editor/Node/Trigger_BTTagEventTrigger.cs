
using System;
using UnityEditor.Experimental.GraphView;
public class Trigger_BTTagEventTrigger : TriggerNode
{
    public override string stateName => "BTTagEventTriggerState";
    public Trigger_BTTagEventTrigger() : base()
    {
        title = "BTTagEventTrigger";

        Port port_exit = CreatePortForNode(this, Direction.Output, typeof(Boolean), Port.Capacity.Multi);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);

    }
}
