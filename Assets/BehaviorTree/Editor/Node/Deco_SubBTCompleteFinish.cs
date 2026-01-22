using UnityEditor.Experimental.GraphView;
public class Deco_SubBTCompleteFinish : DecoratorNode
{
    public override string stateName => "SubBTCompleteFinishState";
    public Deco_SubBTCompleteFinish() : base()
    {
        title = "SubBTCompleteFinish";

        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(System.Boolean), Port.Capacity.Multi);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);


    }
}
