using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace Common.States
{
    public partial class StateMachine<TState, TCommand> : IDisposable
        where TState : unmanaged, Enum
        where TCommand : unmanaged, Enum
    {
        public delegate void ChangeState(TState state, TCommand command);
        
        public event ChangeState OnChanged;
        public event ChangeState OnChanging;
        public TState CurrentState => m_CurrentState.State;
        public TState PreviousState => m_PreviousState.State;

        private Configuration m_Configuration;
        public Configuration Configure 
        { 
            get {
                if (m_Configuration == null)
                    m_Configuration = new Configuration(this);
                return m_Configuration;
            }
        }

        private readonly Dictionary<TState, StateInfo> m_States = new Dictionary<TState, StateInfo>();
        private StateInfo m_CurrentState;
        private StateInfo m_PreviousState;
        private SynchronizationContext m_SynchronizationContext;
        private int m_ThreadID;

        public StateMachine()
        {
            foreach (TState iter in Enum.GetValues(typeof(TState)))
            {
                m_States.Add(iter, new StateInfo(iter));
            }
        }

        public void Init(TState init)
        {
            m_SynchronizationContext = SynchronizationContext.Current;
            m_ThreadID = Thread.CurrentThread.ManagedThreadId;
            m_CurrentState = m_States[init];
        }

        void IDisposable.Dispose()
        {
            foreach (var iter in m_States.Values)
                iter.StateObject?.Dispose();
        }

        public Task SendCommand<TData>(TCommand command, TData data)
        {
            return SendCommand(new InternalCommand(command, data));
        }

        public Task SendCommand(TCommand command)
        {
            return SendCommand(new InternalCommand(command, null));
        }

        private Task SendCommand(InternalCommand command)
        {
            return InternalSendCommand(command);
        }

        private async Task InternalSendCommand(InternalCommand command)
        {
            if (m_ThreadID != Thread.CurrentThread.ManagedThreadId)
            {
                Task result = null;
                m_SynchronizationContext.Send(
                    (o) =>
                    {
                        result = InternalSendCommand(command);
                    }, null);
             
                await result;
                return;
            }

            if (m_CurrentState.TryGetDestination(command, out StateInfo state))
            {
                TCommand cmd = command != null ? command.Command : default;

                OnChanging?.Invoke(state.State, cmd);
                try
                {
                    m_CurrentState.Exit();
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogException(ex);
                    throw;
                }
                
                m_PreviousState = m_CurrentState;
                await m_PreviousState.Execute(state, command);

                m_CurrentState = state;
                OnChanged?.Invoke(CurrentState, cmd);
                try
                {
                    m_CurrentState.Enter(command);
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogException(ex);
                    throw;
                }
                await InternalSendCommand(null);
            }
        }

        private class InternalCommand
        {
            public object Data { get; }
            public TCommand Command { get; }

            public InternalCommand(TCommand command, object data)
            {
                Command = command;
                Data = data;
            }
        }
    }
}