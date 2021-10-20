public class DefaultTransition : Transition {
    public DefaultTransition() : base() {}
    public override float getValue()
    {
        return To;
    }
}