using System;
using System.Threading.Tasks;

namespace Common.Core.Loading
{
    public interface ILoadingCommand
    {
        float GetProgress();
        Task Execute(ILoadingManager manager);
    }
}