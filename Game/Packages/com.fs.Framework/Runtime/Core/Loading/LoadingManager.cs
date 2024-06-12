using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Common.Core.Loading
{
    using Progress;
    
    public sealed class LoadingManager: ILoadingManager
    {
        private readonly List<CommandItem> m_Commands;
        private MultiProgress m_Progress;
        private bool m_IsComplete;
        private Action m_OnComplete;

        private readonly IContainer m_Container;
        
        public LoadingManager(IContainer container, IEnumerable<CommandItem> commands)
        {
            m_Container = container;
            m_Commands = commands.ToList();
            m_Progress = new MultiProgress(new ILoadingCommand[] { });
        }

        #region ILoadingManager
        string ILoadingManager.Text { get; }
        
        IProgress ILoadingManager.Progress => m_Progress;
        
        bool ILoadingManager.IsComplete => m_IsComplete;
        
        Task ILoadingManager.Start(Action onLoadComplete)
        {
            m_OnComplete = onLoadComplete;
            m_IsComplete = false;
            m_Progress = new MultiProgress(m_Commands.Select(iter => iter.Command).ToArray());
            EventWaitHandle @event = new AutoResetEvent(true);

            return Task.Run(
                () =>
                {
                    //TODO: додати обробку помилок у потоці
                    List<CommandItem> commands = new (m_Commands);
                    Prepare();
                    int count = commands.Count;
                    while (count > 0)
                    {
                        while (GetNextCommand(out var command))
                        {
                            command.Execute(this)
                                .ContinueWith(task =>
                                {
                                    if (task.Status == TaskStatus.Faulted)
                                    {
                                        Debug.unityLogger.LogException(task.Exception);
                                    }
                                    m_Progress.SetProgress(command, 1);
                                    RemoveDependency(command);
                                    count--;
                                    @event.Set();
                                });
                        }

                        while (!@event.WaitOne(10) && count > 0)
                        {
                            foreach (var command in commands)
                            {
                                m_Progress.SetProgress(command.Command, command.Command.GetProgress());
                            }
                        } 
                    }

                    m_Progress.SetDone();
                    m_IsComplete = true;
                    onLoadComplete?.Invoke();


                    bool GetNextCommand(out ILoadingCommand command)
                    {
                        var item = commands.FirstOrDefault((iter) => !iter.Dependency.HasDependency);
                        if (item != null) commands.Remove(item);
                        command = item?.Command;
                        return item != null;
                    }

                    void RemoveDependency(ILoadingCommand command)
                    {
                        foreach (var iter in commands)
                            iter.Dependency.Remove(command);
                    }

                    void Prepare()
                    {
                        foreach (var iter in commands)
                        {
                            try
                            {
                                m_Container.Inject(iter.Command);
                                iter.Dependency.Rebuild(this);
                            }
                            catch (Exception e)
                            {
                                Debug.unityLogger.LogException(e);
                            }
                        }
                    }
                });
        }
        #endregion

        [Serializable]
        public struct Dependency
        {
            [SerializeField]
            private int[] m_CommandsIndex;

            private HashSet<ILoadingCommand> m_Commands;

            public bool HasDependency => m_Commands?.Count > 0;

            public void Rebuild(LoadingManager manager)
            {
                if (m_CommandsIndex == null || m_CommandsIndex.Length == 0)
                    return;
                m_Commands = new HashSet<ILoadingCommand>();
                foreach (int iter in m_CommandsIndex)
                    Add(manager.m_Commands[iter].Command);
            }

            public void Add(ILoadingCommand command) 
            {
                m_Commands?.Add(command);
            }
            
            public void Remove(ILoadingCommand command) 
            {
                m_Commands?.Remove(command);
            }
        }

        [Serializable]
        public class CommandItem
        {
            [SerializeReference, ReferenceSelect(typeof(ILoadingCommand))]
            public ILoadingCommand Command;

            [SerializeField]
            public Dependency Dependency;
        }
    }
}