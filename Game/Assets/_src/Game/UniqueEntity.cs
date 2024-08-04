using Common.Core;

using Unity.Entities;
using Unity.Properties;

namespace Game.Model
{
    public readonly struct UniqueEntity : IUniqueEntity, IComponentData, IStorageData
    {
        private readonly Uuid m_Id;
        public Uuid ID => m_Id;

        [CreateProperty] public string DebugId => m_Id.ToString();

        public UniqueEntity(Uuid id)
        {
            m_Id = id;
        }
    }
}
