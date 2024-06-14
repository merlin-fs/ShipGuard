using System.Threading.Tasks;

namespace Common.Core.Loading
{
    public interface ICommand
    {
        Task Execute();
    }
}