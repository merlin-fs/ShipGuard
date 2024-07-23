using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Assertions;

namespace Common.Repositories
{
    using Core;

    [Serializable]
    public sealed class DictionaryRepository<TID, TEntity, TAttr> : IRepository<TID, TEntity, TAttr>
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
            var obj = FindByID(id);
            if (obj is ICastObject castObject)
                return castObject.Cast<T>();
            else
                return (T)obj;
        }
        
        public TEntity FindByID(TID id)
        {
            return m_Items.TryGetValue(id, out TAttr value) 
                ? value.Entity
                : default;
        }

        public void Insert(TID id, TAttr entity)
        {
            var result = m_Items.TryAdd(id, entity);
            Assert.IsTrue(result, $"[Repo: {typeof(TAttr)}] Insert duplicate {id}");
        }

        public void Insert(params (TID, TAttr)[] entities)
        {
            foreach (var (id, entity) in entities)
            {
                m_Items.TryAdd(id, entity);
            }
        }
        
        public void Update(params (TID, TAttr)[] entities)
        {
            foreach (var (id, entity) in entities)
                m_Items[id] = entity;

        }
        public void Remove(params (TID, TAttr)[] entities)
        {
            foreach (var (id, entity) in entities)
                m_Items.TryRemove(id, out TAttr _);
        }

        public IEnumerable<TEntity> Remove(params TID[] ids)
        {
            List<TEntity> result = new List<TEntity>();
            foreach (var id in ids)
            {
                if (!m_Items.TryGetValue(id, out TAttr found)) continue;
                
                result.Add(found.Entity);
                m_Items.TryRemove(id, out TAttr _);
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

