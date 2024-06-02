using System;
using System.Threading.Tasks;


namespace Common.States
{
    public partial class StateMachine<TState, TCommand>
        where TState : unmanaged, Enum
        where TCommand : unmanaged, Enum
    {

        public class ConfigTransition: Configuration
        {
            private readonly StateInfo m_FromInfo;
            private readonly StateInfo m_ToInfo;

            public delegate Task TransitionAction();
            public delegate Task TransitionAction<TData>(TData data);


            internal ConfigTransition(StateMachine<TState, TCommand> owner, TState from, TState to, TCommand command): base(owner)
            {
                m_FromInfo = m_Owner.m_States[from];
                m_ToInfo = m_Owner.m_States[to];
                m_FromInfo.SetDestination(command, m_ToInfo);
            }

            internal ConfigTransition(StateMachine<TState, TCommand> owner, TState from, TState to) : base(owner)
            {
                m_FromInfo = m_Owner.m_States[from];
                m_ToInfo = m_Owner.m_States[to];
                m_FromInfo.SetDestination(null, m_ToInfo);
            }

            public ConfigTransition Action(ITransition value)
            {
                m_FromInfo.SetAction(m_ToInfo.State, value);
                return this;
            }

            public ConfigTransition Action(TransitionAction value)
            {
                m_FromInfo.SetAction(m_ToInfo.State, value);
                return this;
            }

            public ConfigTransition Action(Action value)
            {
                m_FromInfo.SetAction(m_ToInfo.State, value);
                return this;
            }

            public ConfigTransition Action<TData>(ITransition<TData> value)
            {
                m_FromInfo.SetAction(m_ToInfo.State, value);
                return this;
            }

            public ConfigTransition Action<TData>(TransitionAction<TData> value)
            {
                m_FromInfo.SetAction(m_ToInfo.State, value);
                return this;
            }
        }
    }
}
