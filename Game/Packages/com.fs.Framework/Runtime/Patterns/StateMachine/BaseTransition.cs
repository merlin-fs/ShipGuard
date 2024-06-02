using System;
using System.Threading.Tasks;

namespace Common.States
{

    public abstract class BaseTransition : ITransition
    {
        Task ITransition.Execute() => Execute();

        public abstract Task Execute();
    }
}