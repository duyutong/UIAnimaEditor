using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
public class Deco_SubBTContainer : DecoratorNode
{
    public override string stateName => "SubBTContainerState";
    public Deco_SubBTContainer() : base()
    {
        title = "SubBTContainer";

        Port port_enter = CreatePortForNode(this, Direction.Input, typeof(System.Boolean), Port.Capacity.Multi);
        port_enter.portName = "enter";
        inputContainer.Add(port_enter);

        Port port_container = CreatePortForNode(this, Direction.Input, typeof(BTTargetContainer), Port.Capacity.Single);
        port_container.portName = "container";
        inputContainer.Add(port_container);


        Port port_exit = CreatePortForNode(this, Direction.Output, typeof(System.Boolean), Port.Capacity.Multi);
        port_exit.portName = "exit";
        outputContainer.Add(port_exit);

        RegisterCallback<MouseDownEvent>(OnMouseDownEvent);
    }

    private void OnMouseDownEvent(MouseDownEvent evt)
    {
        if (evt.button == (int)MouseButton.LeftMouse && evt.clickCount == 2)
        {
            SubBTContainerState state = btState as SubBTContainerState;
            if (state == null) return;
            if (state.container == null) return;
            if (state.container.target == null) return;

            BehaviourTreeEditor editor = BehaviourTreeEditor.OpenWindow();

            if (btState.runtime == null) editor.OpenBTAsset(state.container.target);
            else editor.LoadRuntimeContainer(btState.runtime);

            evt.StopPropagation();
        }
    }
}
