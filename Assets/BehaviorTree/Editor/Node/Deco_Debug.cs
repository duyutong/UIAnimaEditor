using UnityEditor.Experimental.GraphView;
public class Deco_Debug : DecoratorNode
{
    public override string stateName => "DebugState";
    public Deco_Debug() : base()
    {
        title = "Debug";

        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(System.Boolean), Port.Capacity.Single);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        Port port_logStr = CreatePortForNode(this, Direction.Input, typeof(System.String), Port.Capacity.Single);
        port_logStr.portName = "logStr";
        inputContainer.Add(port_logStr);

        Port port_exit = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);
    }
}
