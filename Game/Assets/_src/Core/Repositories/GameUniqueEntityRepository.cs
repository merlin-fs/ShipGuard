using Common.Core;
using Common.Repositories;

using Game.Model;

using UnityEngine;

namespace Game.Core.Repositories
{
    public class GameUniqueEntityRepository: Repository<Uuid, IGameEntity, GameUniqueEntityRepository.Attribute>
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
            Debug.Log($"[GameEntityRepository] Insert {id}");
            m_Repo.Insert(id, new Attribute(@object));
        }

        public void Remove(Uuid id)
        {
            Debug.Log($"[GameEntityRepository] Remove {id}");
            m_Repo.Remove(id);
        }

        public void Remove(params Uuid[] ids)
        {
            Debug.Log($"[GameEntityRepository] Remove {ids}");
            m_Repo.Remove(ids);
        }
    }
}
