using System;
using System.Threading;
using UnityEngine;

namespace Common.States
{
    public partial class StateMachine<TState, TCommand>
        where TState : unmanaged, Enum
        where TCommand : unmanaged, Enum
    {
        public delegate void StateEvent(StateMachine<TState, TCommand> sender, TState state);

        public partial class Configuration
        {
            protected readonly StateMachine<TState, TCommand> m_Owner;

            internal Configuration(StateMachine<TState, TCommand> owner)
            {
                m_Owner = owner;
            }

            public ConfigTransition Transition(TState from, TState to, TCommand command)
            {
                return new ConfigTransition(m_Owner, from, to, command);
            }

            public ConfigTransition Transition(TState from, TState to)
            {
                return new ConfigTransition(m_Owner, from, to);
            }

            public Configuration Assign(TState state, IState value)
            {
                m_Owner.m_States[state].SetStateObject(value, null);
                return this;
            }

            public Configuration Assign<TData>(TState state, IState<TData> value)
            {
                m_Owner.m_States[state].SetStateObject(value, typeof(TData));
                return this;
            }

        }
    }
}