using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using Unity.Entities;

namespace Game.Storages
{
    public class StorageManager
    {
        private readonly Dictionary<Type, IStorageEndpoint> m_StorageEndpoints = new();

        public void SetContainerEndpoint<T>(IStorageEndpoint endpoint)
            where T : IStorageContainerType
        {
            m_StorageEndpoints[typeof(T)] = endpoint;
        }

        public Task SaveContainer<T>()
            where T : IStorageContainerType
        {
            var type = typeof(T);
            return SaveContainer(m_StorageEndpoints[type], type);
        }

        private Task SaveContainer(IStorageEndpoint endpoint, Type storageContainerType)
        {
            return UniTask.RunOnThreadPool(async () =>
            {
                using var writer = endpoint.GetWriter();
                await UniTask.SwitchToMainThread();
                using var serializer = Serializer.Setup(storageContainerType);
                serializer.SerializeWorld(World.DefaultGameObjectInjectionWorld, writer);
            }).AsTask();
        }
        
        public Task Save()
        {
            return Task.WhenAll(m_StorageEndpoints.Select(iter => SaveContainer(iter.Value, iter.Key)));
        }
        
        public Task LoadContainer<T>()
            where T : IStorageContainerType
        {
            var type = typeof(T);
            return SaveContainer(m_StorageEndpoints[type], type);
        }

        private Task LoadContainer(IStorageEndpoint endpoint, Type storageContainerType)
        {
            return UniTask.RunOnThreadPool(async () =>
            {
                using var reader = endpoint.GetReader();
                await UniTask.SwitchToMainThread();
                using var serializer = Serializer.Setup(storageContainerType);
                serializer.DeserializeWorld(World.DefaultGameObjectInjectionWorld, reader);
            }).AsTask();
        }
        
        public Task Load()
        {
            return Task.WhenAll(m_StorageEndpoints.Select(iter => LoadContainer(iter.Value, iter.Key)));
        }
    }
}
