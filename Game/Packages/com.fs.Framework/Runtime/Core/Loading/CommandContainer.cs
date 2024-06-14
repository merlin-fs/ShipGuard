using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Common.Core.Progress;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Common.Core.Loading
{
    public sealed class CommandContainer: ICommandProgress, ICommandNewContainer
    {
        private MultiProgress m_Progress;
        private readonly List<ICommandItem> m_Commands = new();
        private IContainer m_Container;
        private Action m_OnComplete;

        public CommandContainer(IContainer container)
        {
            m_Container = container;
        }

        public CommandContainer Add(params ICommandItem[] items)
        {
            m_Commands.AddRange(items);
            return this;
        }

        public CommandContainer WithCallback(Action onComplete)
        {
            m_OnComplete = onComplete;
            return this;
        }

        public float GetProgress() => ((IProgress)m_Progress).Value;
        public IContainer GetContainer() => m_Container;

        public Task Execute()
        {
            m_Progress = new MultiProgress(m_Commands
                .Where(iter => iter.Command is ICommandProgress)
                .Select(iter => iter.Command)
                .ToArray());
            EventWaitHandle @event = new AutoResetEvent(true);
            var injector = m_Container;
            
            return Task.Run( 
                () =>
                {
                    List<CommandItem> commands = new (m_Commands.Select(iter => new CommandItem(iter)));
                    Prepare(commands.Select(iter => iter.Command).ToArray());
                    
                    int count = commands.Count;
                    while (count > 0)
                    {
                        while (GetNextCommand(out var command))
                        {
                            UnityMainThread.Context.Send(_ =>
                            {
                                try
                                {
                                    injector.Inject(command);
                                }
                                catch (Exception e)
                                {
                                    Debug.unityLogger.LogException(e);
                                }
                            }, default);
                            command.Execute()
                                .ContinueWith(task =>
                                {
                                    if (task.Status == TaskStatus.Faulted)
                                    {
                                        Debug.unityLogger.LogException(task.Exception);
                                    }
                                    
                                    m_Progress.SetProgress(command, 1);
                                    RemoveDependency(command);
                                    
                                    if (command is ICommandNewContainer newContainer)
                                        injector = m_Container = newContainer.GetContainer();
                                    
                                    Next();
                                });
                        }
                        
                        while (!@event.WaitOne(10) && count > 0)
                        {
                            m_Progress.SetProgressPopulate(command => ((ICommandProgress)command).GetProgress());
                        } 
                    }

                    m_Progress.SetDone();
                    m_OnComplete?.Invoke();

                    void Next()
                    {
                        count--;
                        @event.Set();
                    }
                    
                    bool GetNextCommand(out ICommand command)
                    {
                        var item = commands.FirstOrDefault(iter => !iter.Dependency.HasDependency);
                        if (item != null) commands.Remove(item);
                        command = item?.Command;
                        return item != null;
                    }

                    void RemoveDependency(ICommand command)
                    {
                        foreach (var iter in commands)
                            iter.Dependency.Remove(command);
                    }

                    void Prepare(ICommand[] sources)
                    {
                        foreach (var iter in commands)
                        {
                            try
                            {
                                iter.Dependency.Rebuild(sources);
                            }
                            catch (Exception e)
                            {
                                Debug.unityLogger.LogException(e);
                            }
                        }
                    }
                });
        }
        
        private class CommandItem
        {
            public readonly ICommand Command;
            public readonly Dependency Dependency;

            public CommandItem(ICommandItem commandItem)
            {
                Command = commandItem.Command;
                Dependency = new Dependency 
                {
                    CommandsIndex = commandItem.Dependencies,
                };
            }
        }

        private class Dependency
        {
            private HashSet<ICommand> m_Commands;

            public int[] CommandsIndex;
            public bool HasDependency => m_Commands?.Count > 0;

            public void Rebuild(IReadOnlyList<ICommand> commands)
            {
                if (CommandsIndex == null || CommandsIndex.Length == 0)
                    return;
                m_Commands = new HashSet<ICommand>();
                foreach (int iter in CommandsIndex)
                    Add(commands[iter]);
            }

            private void Add(ICommand command)
            {
                m_Commands?.Add(command);
            }

            public void Remove(ICommand command)
            {
                m_Commands?.Remove(command);
            }
        }
    }
}