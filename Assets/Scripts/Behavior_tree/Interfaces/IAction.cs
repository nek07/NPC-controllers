public interface IAction
{
    NodeState Execute(ActionContext context);
}