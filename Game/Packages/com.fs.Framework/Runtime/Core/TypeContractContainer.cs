using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Core
{
    public class TypeContractContainer
    {
        private readonly Dictionary<Type, List<TypeContract>> m_Contracts = new();

        public TypeContract Bind<TContract, TContractValue>()
        {
            return Bind(typeof(TContract), typeof(TContractValue));
        }

        public TypeContract Bind(Type type, Type contractType)
        {
            if (!m_Contracts.TryGetValue(type, out var contract))
            {
                m_Contracts[type] = new List<TypeContract>();
            }

            var newContract = new TypeContract(type, contractType);
            m_Contracts[type].Add(newContract);
            return newContract;
        }

        public IEnumerable<Type> GetContractType(Type contactKey)
        {
            return GetContract(contactKey).Select(iter => iter.ContractType);
        }

        public IEnumerable<TypeContract> GetContract(Type contactKey)
        {
            if (!m_Contracts.TryGetValue(contactKey, out var contracts) || contracts.Count == 0)
            {
                yield break;
            }

            foreach (var iter in contracts)
            {
                yield return iter;
            }
        }

        public void Clear()
        {
            m_Contracts.Clear();
        }
    }
}