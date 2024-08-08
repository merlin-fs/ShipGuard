using Common.Core;
using Common.Repositories;

using Game.Model;
using Game.Views;

namespace Game.Core.Repositories
{
    public class InteractionViewRepository: Repository<int, IView, InteractionViewRepository.Attribute>
    {
        public class Attribute : IEntityAttributes<IView>
        {
            public IView Entity { get; }
            public Attribute(IView entity)
            {
                Entity = entity;
            }
        }
        
        public void Remove(int id)
        {
            m_Repo.Remove(id);
        }
        
        public void Insert(int id, IView @object)
        {
            m_Repo.Insert(id, new Attribute(@object));
        }
    }
}
