using UnityEditor.Experimental.GraphView;
public class Behav_MoveTo : BehaviorNode
{
    public override string stateName => "MoveToState";
    public Behav_MoveTo() : base()
    {
        title = "MoveTo";

        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(System.Boolean), Port.Capacity.Single);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);


        Port port_exit = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);

    }
}
