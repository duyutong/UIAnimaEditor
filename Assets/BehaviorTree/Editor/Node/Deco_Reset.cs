using UnityEditor.Experimental.GraphView;
public class Deco_Reset : DecoratorNode
{
    public override string stateName => "ResetState";
    public Deco_Reset() : base()
    {
        title = "Reset";

        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(System.Boolean), Port.Capacity.Single);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);


    }
}
