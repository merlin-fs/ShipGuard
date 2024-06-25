using Common.Core;
using Common.Repositories;

using Game.Model;

namespace Game.Core.Repositories
{
    public class LocationRepository: Repository<Uuid, IGameEntity, LocationRepository.Attribute>
    {
        public class Attribute : IEntityAttributes<IGameEntity>
        {
            public IGameEntity Entity { get; }
            public Attribute(IGameEntity entity)
            {
                Entity = entity;
            }
        }
        
        public void Insert(Uuid id, IGameEntity @object)
        {
            m_Repo.Insert(id, new Attribute(@object));
        }
    }
}
