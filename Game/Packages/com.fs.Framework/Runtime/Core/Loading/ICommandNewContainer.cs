namespace Common.Core.Loading
{
    public interface ICommandNewContainer: ICommand
    {
        IContainer GetContainer();
    }
}