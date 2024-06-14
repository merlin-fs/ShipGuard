using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Core.Loading
{
    public interface ILoadingManager
    {
        IProgress Progress { get; }
        string Text { get; }
        Task Start(IContainer container, params ICommandItem[] commands);
    }
}
