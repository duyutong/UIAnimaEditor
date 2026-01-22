using UnityEditor.Experimental.GraphView;
public class Behav_SnapArea2D : BehaviorNode
{
    public override string stateName => "SnapArea2DState";
    public Behav_SnapArea2D() : base()
    {
        title = "SnapArea2D";

        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(System.Boolean), Port.Capacity.Single);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        Port port_animaTime = CreatePortForNode(this, Direction.Input, typeof(System.Single), Port.Capacity.Single);
        port_animaTime.portName = "animaTime";
        inputContainer.Add(port_animaTime);

        Port port_areaObj = CreatePortForNode(this, Direction.Input, typeof(BTTargetObject), Port.Capacity.Single);
        port_areaObj.portName = "areaObj";
        inputContainer.Add(port_areaObj);

        Port port_checkObj = CreatePortForNode(this, Direction.Input, typeof(BTTargetObject), Port.Capacity.Single);
        port_checkObj.portName = "checkObj";
        inputContainer.Add(port_checkObj);


        Port port_isInSnapArea = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Single);
        port_isInSnapArea.portName = "isInSnapArea";
        outputContainer.Add(port_isInSnapArea);

        Port port_isOutOfSnapArea = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Single);
        port_isOutOfSnapArea.portName = "isOutOfSnapArea";
        outputContainer.Add(port_isOutOfSnapArea);

    }
}
