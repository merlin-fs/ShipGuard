using System;
using System.Collections.Generic;
using System.Linq;

using Common.Core;

using Reflex.Exceptions;
using Reflex.Extensions;
using Reflex.Generics;
using Reflex.Injectors;
using Reflex.Logging;
using Reflex.Resolvers;

namespace Reflex.Core
{
    public sealed class Container : IDisposable, IContainer
    {
        private readonly DisposableCollection _disposables;

        public string Name { get; }
        internal Container Parent { get; }
        internal List<Container> Children { get; } = new();
        internal Dictionary<Type, List<IResolver>> ResolversByContract { get; }
        
        internal Container(string name, Container parent, Dictionary<Type, List<IResolver>> resolversByContract, DisposableCollection disposables)
        {
            Diagnosis.RegisterBuildCallSite(this);
            Name = name;
            Parent = parent;
            Parent?.Children.Add(this);
            ResolversByContract = resolversByContract;
            _disposables = disposables;
            OverrideSelfInjection();
        }

        public bool HasBinding<T>()
        {
            return HasBinding(typeof(T));
        }

        public bool HasBinding(Type type)
        {
            return ResolversByContract.ContainsKey(type);
        }

        public void Dispose()
        {
            foreach (var child in Children.Reversed())
            {
                child.Dispose();
            }

            Parent?.Children.Remove(this);
            ResolversByContract.Clear();
            _disposables.Dispose();
            ReflexLogger.Log($"Container {Name} disposed", LogLevel.Info);
        }

        public Container Scope(Action<ContainerBuilder> extend = null)
        {
            var builder = new ContainerBuilder().SetParent(this);
            extend?.Invoke(builder);
            return builder.Build();
        }
        
        public T Construct<T>()
        {
            return (T)Construct(typeof(T));
        }

        public T Construct<T>(params object[] arguments)
        {
            return (T)Construct(typeof(T), arguments);
        }

        public object Construct(Type concrete)
        {
            var instance = ConstructorInjector.Construct(concrete, this);
            AttributeInjector.Inject(instance, this);   
            return instance;
        }

        public object Construct(Type concrete, params object[] arguments)
        {
            var instance = ConstructorInjector.Construct(concrete, arguments);
            AttributeInjector.Inject(instance, this);   
            return instance;
        }

        public TContract ResolveId<TContract>(object identifier)
        {
            throw new NotImplementedException();
        }

        public object Resolve(Type type)
        {
            if (type.IsEnumerable(out var elementType))
            {
                return All(elementType).CastDynamic(elementType);
            }

            var resolvers = GetResolvers(type);
            var lastResolver = resolvers.Last();
            var resolved = lastResolver.Resolve(this);
            return resolved;
        }

        public object ResolveId(Type contractType, object identifier)
        {
            throw new NotImplementedException();
        }

        public void Inject(object obj)
        {
            AttributeInjector.Inject(obj, this);   
        }

        public T Instantiate<T>()
        {
            return Construct<T>();
        }

        public T Instantiate<T>(IEnumerable<object> extraArgs)
        {
            return Construct<T>(extraArgs);
        }

        public object Instantiate(Type concreteType)
        {
            return Construct(concreteType);
        }

        public object Instantiate(Type concreteType, IEnumerable<object> extraArgs)
        {
            return Construct(concreteType, extraArgs);
        }

        public TContract Resolve<TContract>()
        {
            return (TContract)Resolve(typeof(TContract));
        }
        
        public object Single(Type type)
        {
            return GetResolvers(type).Single().Resolve(this);
        }

        public TContract Single<TContract>()
        {
            return (TContract)Single(typeof(TContract));
        }

        public IEnumerable<object> All(Type contract)
        {
            return ResolversByContract.TryGetValue(contract, out var resolvers)
                ? resolvers.Select(resolver => resolver.Resolve(this)).ToArray()
                : Enumerable.Empty<object>();
        }

        public IEnumerable<TContract> All<TContract>()
        {
            return ResolversByContract.TryGetValue(typeof(TContract), out var resolvers)
                ? resolvers.Select(resolver => (TContract) resolver.Resolve(this)).ToArray()
                : Enumerable.Empty<TContract>();
        }

        private IEnumerable<IResolver> GetResolvers(Type contract)
        {
            if (ResolversByContract.TryGetValue(contract, out var resolvers))
            {
                return resolvers;
            }

            throw new UnknownContractException(contract);
        }
        
        private void OverrideSelfInjection()
        {
            ResolversByContract[typeof(Container)] = new List<IResolver> { new SingletonValueResolver(this) };
        }
    }
}