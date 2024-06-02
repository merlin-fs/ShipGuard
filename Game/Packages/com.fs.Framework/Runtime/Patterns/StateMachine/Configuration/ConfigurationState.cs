using System;
using System.Threading.Tasks;

namespace Common.States
{
    public partial class StateMachine<TState, TCommand>
        where TState : unmanaged, Enum
        where TCommand : unmanaged, Enum
    {
        /*
        public partial class ConfigurationState : Configuration
        {
            public Configuration Configure => m_Owner.Configure;

            private readonly StateMachine<TState, TCommand> m_Owner;
            private readonly TState m_State;

            internal ConfigurationState(StateMachine<TState, TCommand> owner, TState state): base(owner)
            {
                m_State = state;
                m_Events = m_Owner.m_States[m_State].StateEvents as InternalState
                    ?? new InternalState(m_Owner, m_State);
            }

            public ConfigurationState OnEnter(StateEvent value)
            {
                //m_Events.OnEnter += value;
                SetEvents();
                return this;
            }

            public ConfigurationState OnExit(StateEvent value)
            {
                //m_Events.OnExit -= value;
                SetEvents();
                return this;
            }

            private void SetEvents()
            {
                //m_Owner.m_States[m_State].StateEvents = m_Events;
            }
        }
        */
    }
}