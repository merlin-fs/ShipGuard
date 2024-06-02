using System;
using System.Threading.Tasks;

namespace Common.States
{
    public interface IBaseTransition
    {

    }

    public interface ITransition: IBaseTransition
    {
        Task Execute();
    }

    public interface ITransition<TData> : IBaseTransition
    {
        Task Execute(TData data);
    }

}