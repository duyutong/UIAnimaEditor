public abstract class DecoratorNode : BehaviorTreeBaseNode
{
    public override string Prefix => "Deco";
    public DecoratorNode() : base()
    {
        title = "*DecoratorNode";
    }
}