using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Common.Repositories
{
    using Core;

    [Serializable]
    public sealed class DictionaryRepository<TID, TEntity, TAttr> : IRepository<TID, TEntity, TAttr>
        where TEntity: IIdentifiable<TID>
        where TAttr : IEntityAttributes<TEntity>
    {
        private readonly ConcurrentDictionary<TID, TAttr> m_Items;

        public DictionaryRepository()
        {
            m_Items = new ConcurrentDictionary<TID, TAttr>();
        }

        public DictionaryRepository(Dictionary<TID, TAttr> items)
        {
            m_Items = new ConcurrentDictionary<TID, TAttr>(items);
        }

        #region IRepository<TID, TEntity>
        public IEnumerable<TEntity> Find(Func<TAttr, bool> filter,
            Func<IQueryable<TAttr>, IOrderedQueryable<TAttr>> orderBy)
        {
            var qry = m_Items.Values.AsEnumerable();

            if (filter != null)
                qry = qry
                    .Where(filter)
                    .AsEnumerable();
            
            if (orderBy != null)
                qry = orderBy?.Invoke(qry.AsQueryable());
            
            return qry.Select(i => i.Entity);
        }

        public T FindByID<T>(TID id) 
            where T : TEntity
        {
            return (T)FindByID(id);
        }
        
        public TEntity FindByID(TID id)
        {
            return m_Items.TryGetValue(id, out TAttr value) 
                ? value.Entity
                : default;
        }

        public void Insert(TID id, TAttr entity, Action<TEntity> callback = null)
        {
            if (m_Items.TryAdd(id, entity))
                callback?.Invoke(entity.Entity);
        }

        public void Insert(IEnumerable<TAttr> entities, Action<TEntity> callback = null)
        {
            foreach (var entity in entities)
            {
                if (m_Items.TryAdd(entity.Entity.ID, entity))
                    callback?.Invoke(entity.Entity);
            }
        }
        
        public void Update(IEnumerable<TAttr> entities)
        {
            foreach (var entity in entities)
                m_Items[entity.Entity.ID] = entity;

        }
        public void Remove(IEnumerable<TAttr> entities, Action<TEntity> callback = null)
        {
            foreach (var entity in entities)
                if (m_Items.TryRemove(entity.Entity.ID, out TAttr _))
                    callback?.Invoke(entity.Entity);
        }

        public IEnumerable<TEntity> Remove(IEnumerable<TID> ids, Action<TEntity> callback = null)
        {
            List<TEntity> result = new List<TEntity>();
            foreach (var id in ids)
            {
                if (m_Items.TryGetValue(id, out TAttr found))
                {
                    result.Add(found.Entity);
                    if (m_Items.TryRemove(id, out TAttr _))
                        callback?.Invoke(found.Entity);
                }
            }
            return result.ToArray();
        }
        #endregion

        public void Clear()
        {
            m_Items.Clear();
        }
    }
}

