public class DefaultNode : BehaviorTreeBaseNode
{
    public string nodeType;
    public override string stateName => "Null";
    public DefaultNode() : base()
    {
        title = "*TempNode";
    }
}
