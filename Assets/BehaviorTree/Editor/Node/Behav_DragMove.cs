
using System;
using UnityEditor.Experimental.GraphView;
public class Behav_DragMove : BehaviorNode
{
    public override string stateName => "DragMoveState";
    public Behav_DragMove() : base()
    {
        title = "DragMove";

        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(Boolean), Port.Capacity.Single);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        Port port_targetObj = CreatePortForNode(this, Direction.Input, typeof(BTTargetObject), Port.Capacity.Single);
        port_targetObj.portName = "targetObj";
        inputContainer.Add(port_targetObj);


        Port port_exit = CreatePortForNode(this, Direction.Output, typeof(Boolean), Port.Capacity.Single);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);

    }
}
