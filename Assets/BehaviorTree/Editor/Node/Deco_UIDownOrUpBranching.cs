using UnityEditor.Experimental.GraphView;
public class Deco_UIDownOrUpBranching : DecoratorNode
{
    public override string stateName => "UIDownOrUpBranchingState";
    public Deco_UIDownOrUpBranching() : base()
    {
        title = "UIDownOrUpBranching";

        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(System.Boolean), Port.Capacity.Single);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        Port port_targetObj = CreatePortForNode(this, Direction.Input, typeof(BTTargetObject), Port.Capacity.Single);
        port_targetObj.portName = "targetObj";
        inputContainer.Add(port_targetObj);

        Port port_pointerDown = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_pointerDown.portName = "pointerDown";
        outputContainer.Add(port_pointerDown);

        Port port_pointerUp = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_pointerUp.portName = "pointerUp";
        outputContainer.Add(port_pointerUp);

        Port port_idel = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_idel.portName = "idel";
        outputContainer.Add(port_idel);

    }
}
