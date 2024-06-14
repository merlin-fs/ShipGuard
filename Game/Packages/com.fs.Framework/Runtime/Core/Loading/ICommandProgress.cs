namespace Common.Core.Loading
{
    public interface ICommandProgress: ICommand
    {
        float GetProgress();
    }
}