using System;
using System.Threading.Tasks;

namespace Common.Core.Loading
{
    public interface ILoadingManager
    {
        IProgress Progress { get; }
        string Text { get; }
        bool IsComplete { get; }
        Task Start(Action onLoadComplete = null);
    }
}
