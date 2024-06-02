using System;

namespace Common.Core
{
    public class TypeContract
    {
        public Type ContractType { get; }
            
        public Func<object, bool> Filter { get; private set; }
        
        public TypeContract(Type type, Type contractType)
        {
            ContractType = contractType;
        }

        public TypeContract WithFilter<T>(Func<T, bool> filter) where T : class
        {
            Filter = o => o is T casted && filter(casted);
            return this;
        }
    }
}