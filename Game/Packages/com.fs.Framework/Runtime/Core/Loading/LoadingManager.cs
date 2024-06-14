using System;
using System.Linq;
using System.Threading.Tasks;

using Common.Core.Progress;

namespace Common.Core.Loading
{
    public sealed class LoadingManager: ILoadingManager
    {
        private IProgress m_Progress;

        public LoadingManager()
        {
            m_Progress = new WrapperProgress(() => 0);
        }

        #region ILoadingManager

        string ILoadingManager.Text { get; }
        
        IProgress ILoadingManager.Progress => m_Progress;
        
        Task ILoadingManager.Start(IContainer container, params ICommandItem[] commands)
        {
            var command = new CommandContainer(container)
                .Add(commands.ToArray())
                .WithCallback(() =>
                {
                    m_Progress = new WrapperProgress(() => 0);
                });
            m_Progress = new WrapperProgress(command.GetProgress);
            return command.Execute();
        }
        #endregion
    }
}