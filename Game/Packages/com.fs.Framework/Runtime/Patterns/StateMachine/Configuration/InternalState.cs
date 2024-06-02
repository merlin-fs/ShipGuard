using System;

using UnityEngine;

namespace Common.States
{
    public partial class StateMachine<TState, TCommand>
        where TState : unmanaged, Enum
        where TCommand : unmanaged, Enum
    {
        public partial class ConfigurationState
        {

            /*
            private class InternalState : IState<TState>
            {
                public event StateEvent OnEnter;
                public event StateEvent OnExit;

                private readonly TState m_State;
                private readonly StateMachine<TState, TCommand> m_Owner;

                public InternalState(StateMachine<TState, TCommand> owner, TState state)
                {
                    m_Owner = owner;
                    m_State = state;
                }

                bool IState<TState>.Validate<TData>(TData data) => true;

                void IState<TState>.Enter() => OnEnter?.Invoke(m_Owner, m_State);
                void IState<TState>.Exit() => OnExit?.Invoke(m_Owner, m_State);
                void IDisposable.Dispose() { }
            }
            */
        }
    }
}