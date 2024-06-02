using System;
using System.Collections.Generic;

namespace Common.Core
{
    public interface IContainer
    {
        T Instantiate<T>();
        T Instantiate<T>(IEnumerable<object> extraArgs);

        object Instantiate(Type concreteType);
        object Instantiate(Type concreteType, IEnumerable<object> extraArgs);
        
        TContract Resolve<TContract>();
        TContract ResolveId<TContract>(object identifier);

        object Resolve(Type contractType);
        object ResolveId(Type contractType, object identifier);

        void Inject(object obj);
    }
}