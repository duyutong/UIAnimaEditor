using UnityEditor.Experimental.GraphView;
public class Behav_SetUILayoutOrder : BehaviorNode
{
    public override string stateName => "SetUILayoutOrderState";
    public Behav_SetUILayoutOrder() : base()
    {
        title = "SetUILayoutOrder";

        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(System.Boolean), Port.Capacity.Single);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        Port port_targetObj = CreatePortForNode(this, Direction.Input, typeof(BTTargetObject), Port.Capacity.Single);
        port_targetObj.portName = "targetObj";
        inputContainer.Add(port_targetObj);

        Port port_sortingOrder = CreatePortForNode(this, Direction.Input, typeof(System.Int32), Port.Capacity.Single);
        port_sortingOrder.portName = "sortingOrder";
        inputContainer.Add(port_sortingOrder);

        Port port_sortingLayerName = CreatePortForNode(this, Direction.Input, typeof(System.String), Port.Capacity.Single);
        port_sortingLayerName.portName = "sortingLayerName";
        inputContainer.Add(port_sortingLayerName);


        Port port_exit = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Single);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);

    }
}
