using System;
using System.Collections.Generic;

using Unity.Entities;

namespace Game.Storages
{
    public interface IStorageContainerType : IStorageData{}
    
    public struct StorageLocation : IStorageContainerType, IComponentData
    {
        
    }

    public struct StorageUser : IStorageContainerType, IComponentData
    {
        
    }
    
    public class StorageDataManager
    {
        private readonly Dictionary<Type, Type> m_TypeContracts = new();
            
        void RegistryData<TStorageType>(params Type[] storageData)
            where TStorageType : IStorageContainerType
        {
            foreach (var data in storageData)
            {
                RegistryData(typeof(TStorageType), data);
            }
        }

        void RegistryData<TStorageType, TStorageData>()
            where TStorageType : IStorageContainerType
            where TStorageData : IStorageData
        {
            RegistryData(typeof(TStorageType), typeof(TStorageData));
        }

        private void RegistryData(Type storageType, Type storageData)
        {
            m_TypeContracts.Add(storageData, storageType);
        }
    }
}
