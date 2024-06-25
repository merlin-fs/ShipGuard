using Common.Core;
using Common.Repositories;

using Game.Model;
using Game.Views;

namespace Game.Core.Repositories
{
    public class LocationViewRepository: Repository<Uuid, IView, LocationViewRepository.Attribute>
    {
        public class Attribute : IEntityAttributes<IView>
        {
            public IView Entity { get; }
            public Attribute(IView entity)
            {
                Entity = entity;
            }
        }
        
        public void Insert(Uuid id, IView @object)
        {
            m_Repo.Insert(id, new Attribute(@object));
        }
    }
}
