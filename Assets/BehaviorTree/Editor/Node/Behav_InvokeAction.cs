using UnityEditor.Experimental.GraphView;
public class Behav_InvokeAction : BehaviorNode
{
    public override string stateName => "InvokeActionState";
    public Behav_InvokeAction() : base()
    {
        title = "InvokeAction";

        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(System.Boolean), Port.Capacity.Single);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        Port port_btEvent = CreatePortForNode(this, Direction.Input, typeof(BTTargetEvent), Port.Capacity.Single);
        port_btEvent.portName = "btEvent";
        inputContainer.Add(port_btEvent);


        Port port_exit = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Single);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);

    }
}
