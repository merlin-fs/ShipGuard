using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;

namespace Common.States
{
    public partial class StateMachine<TState, TCommand>
        where TState : unmanaged, Enum
        where TCommand : unmanaged, Enum
    {
        private class StateInfo
        {
            public TState State { get; }
            public IBaseState StateObject => m_StateObject.State;

            private readonly Dictionary<TCommand, StateInfo> m_DestinationStates = new Dictionary<TCommand, StateInfo>();
            private StateInfo m_DestinationDefault = null;
            private readonly Dictionary<TState, InternalTransition> m_Transitions = new Dictionary<TState, InternalTransition>();
            private InternalStateObject m_StateObject;


            public StateInfo(TState state)
            {
                State = state;
            }

            public void SetDestination(TCommand? command, StateInfo to)
            {
                if (command.HasValue)
                    m_DestinationStates[command.Value] = to;
                else
                    m_DestinationDefault = to;
            }

            public void SetStateObject(IBaseState value, Type type)
            {
                m_StateObject.State = value;
                m_StateObject.Type = type;
                m_StateObject.Enter = value.GetType().GetMethod("Enter");
                m_StateObject.Exit = value.GetType().GetMethod("Exit");
            }

            public void SetAction(TState state, ITransition transition)
            {
                m_Transitions[state] = new InternalTransition()
                {
                    Transition = transition,
                    Type = null,
                };
            }

            public void SetAction(TState state, Action transition)
            {
                SetAction(state,
                    () =>
                    {
                        transition.Invoke();
                        return Task.CompletedTask;
                    });
            }

            public void SetAction<TData>(TState state, ITransition<TData> transition)
            {
                m_Transitions[state] = new InternalTransition()
                {
                    Transition = transition,
                    Type = typeof(TData),
                };
            }

            public void SetAction(TState state, ConfigTransition.TransitionAction transition)
            {
                m_Transitions[state] = new InternalTransition()
                {
                    Transition = new Transition(transition),
                    Type = null,
                };
            }

            public void SetAction<TData>(TState state, ConfigTransition.TransitionAction<TData> transition)
            {
                m_Transitions[state] = new InternalTransition()
                {
                    Transition = new Transition<TData>(transition),
                    Type = typeof(TData),
                };
            }

            public bool TryGetDestination(InternalCommand command, out StateInfo state)
            {
                if (command != null)
                    return m_DestinationStates.TryGetValue(command.Command, out state);
                else
                {
                    state = m_DestinationDefault;
                    return (m_DestinationDefault != null);
                }
            }

            public void Enter(InternalCommand command)
            {
                if (m_StateObject.State == null)
                    return;

                object[] data = null;
                if (m_StateObject.Type != null)
                {
                    data = new object[] { command.Data };
                }
                m_StateObject.Enter.Invoke(StateObject, data);
                m_StateObject.EnterCommand = command;
            }

            public void Exit()
            {
                var cmd = m_StateObject.EnterCommand;
                m_StateObject.EnterCommand = default;
                Exit(cmd);
            }

            private void Exit(InternalCommand command)
            {
                if (m_StateObject.State == null)
                    return;

                object[] data = null;
                if (m_StateObject.Type != null)
                {
                    data = new object[] { command?.Data };
                }
                m_StateObject.Exit.Invoke(StateObject, data);
            }

            public Task Execute(StateInfo state, InternalCommand command)
            {
                if (m_Transitions.TryGetValue(state.State, out InternalTransition transition))
                {
                    MethodInfo method = transition.Transition.GetType().GetMethod("Execute");
                    object[] data = null;
                    if (transition.Type != null)
                    {
                        data = new object[] { command.Data };
                    }
                    return (Task)method.Invoke(transition.Transition, data);
                }
                return Task.CompletedTask;
            }

            private class Transition : ITransition
            {
                private ConfigTransition.TransitionAction m_Action;

                public Transition(ConfigTransition.TransitionAction action)
                {
                    m_Action = action;
                }

                public Task Execute() => m_Action.Invoke();
            }

            private class Transition<TData> : ITransition<TData>
            {
                private ConfigTransition.TransitionAction<TData> m_Action;

                public Transition(ConfigTransition.TransitionAction<TData> action)
                {
                    m_Action = action;
                }

                public Task Execute(TData data) => m_Action.Invoke(data);
            }

            private struct InternalTransition
            {
                public IBaseTransition Transition;
                public Type Type;
            }

            private struct InternalStateObject
            {
                public IBaseState State;
                public Type Type;
                public MethodInfo Enter;
                public MethodInfo Exit;
                public InternalCommand EnterCommand;
            }
        }
    }
}