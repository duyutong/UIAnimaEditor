using UnityEditor.Experimental.GraphView;
public class Deco_UIHoverBranching : DecoratorNode
{
    public override string stateName => "UIHoverBranchingState";
    public Deco_UIHoverBranching() : base()
    {
        title = "UIHoverBranching";

        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(System.Boolean), Port.Capacity.Single);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        Port port_targetObj = CreatePortForNode(this, Direction.Input, typeof(BTTargetObject), Port.Capacity.Single);
        port_targetObj.portName = "targetObj";
        inputContainer.Add(port_targetObj);

        Port port_uiCameraObj = CreatePortForNode(this, Direction.Input, typeof(BTTargetObject), Port.Capacity.Single);
        port_uiCameraObj.portName = "uiCameraObj";
        inputContainer.Add(port_uiCameraObj);


        Port port_pointerEnter = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_pointerEnter.portName = "pointerEnter";
        outputContainer.Add(port_pointerEnter);

        Port port_hover = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_hover.portName = "hover";
        outputContainer.Add(port_hover);

        Port port_pointerExit = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_pointerExit.portName = "pointerExit";
        outputContainer.Add(port_pointerExit);

        Port port_idel = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_idel.portName = "idel";
        outputContainer.Add(port_idel);

    }
}
