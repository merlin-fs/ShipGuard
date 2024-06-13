using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Core.Loading
{
    public interface ILoadingManager
    {
        IProgress Progress { get; }
        string Text { get; }
        bool IsComplete { get; }
        Task Start(IContainer container, IEnumerable<LoadingManager.CommandItem> commands, Action onLoadComplete = null);
    }
}
