using System;

namespace Common.States
{
    public abstract class BaseState : IState
    {
        public virtual void Dispose() { }
        public virtual void Enter() { }
        public virtual void Exit() { }
    }

    public abstract class BaseState<TData> : IState<TData>
    {
        public virtual void Dispose() { }
        public virtual void Enter(TData data) { }
        public virtual void Exit(TData data) { }
    }
}