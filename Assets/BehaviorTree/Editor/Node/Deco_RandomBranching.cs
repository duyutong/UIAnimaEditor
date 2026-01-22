using UnityEditor.Experimental.GraphView;
public class Deco_RandomBranching : DecoratorNode
{
    public override string stateName => "RandomBranchingState";
    public Deco_RandomBranching() : base()
    {
        title = "RandomBranching";

        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(System.Boolean), Port.Capacity.Single);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);


        Port port_exit = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);

    }
}
