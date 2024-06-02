using System;

namespace Common.States
{
    public interface IBaseState : IDisposable
    {
    }

    public interface IState : IBaseState
    {
        void Enter();
        void Exit();
    }

    public interface IState<TData> : IBaseState
    {
        void Enter(TData arg1);
        void Exit(TData arg1);
    }
}