using UnityEditor.Experimental.GraphView;
public class Deco_UIDragBranching : DecoratorNode
{
    public override string stateName => "UIDragBranchingState";
    public Deco_UIDragBranching() : base()
    {
        title = "UIDragBranching";

        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(System.Boolean), Port.Capacity.Single);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        Port port_targetObj = CreatePortForNode(this, Direction.Input, typeof(BTTargetObject), Port.Capacity.Single);
        port_targetObj.portName = "targetObj";
        inputContainer.Add(port_targetObj);

        Port port_uiCameraObj = CreatePortForNode(this, Direction.Input, typeof(BTTargetObject), Port.Capacity.Single);
        port_uiCameraObj.portName = "uiCameraObj";
        inputContainer.Add(port_uiCameraObj);


        Port port_beginDrag = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_beginDrag.portName = "beginDrag";
        outputContainer.Add(port_beginDrag);

        Port port_drag = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_drag.portName = "drag";
        outputContainer.Add(port_drag);

        Port port_endDrag = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_endDrag.portName = "endDrag";
        outputContainer.Add(port_endDrag);

        Port port_idel = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_idel.portName = "idel";
        outputContainer.Add(port_idel);

    }
}
