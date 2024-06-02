using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Repositories
{
    public interface IEntityAttributes<T>
    {
        T Entity { get; }
    }

    public interface IReadonlyRepository<TID, TEntity, TAttr>
        where TAttr : IEntityAttributes<TEntity>
    {
        IEnumerable<TEntity> Find(Func<TAttr, bool> filter = null, Func<IQueryable<TAttr>, IOrderedQueryable<TAttr>> orderBy = null);
        TEntity FindByID(TID id);
        T FindByID<T>(TID id) where T : TEntity;
    }

    public interface IRepository<TID, TEntity, TAttr>: IReadonlyRepository<TID, TEntity, TAttr>
        where TAttr : IEntityAttributes<TEntity>
    {
        void Insert(IEnumerable<TAttr> entities, Action<TEntity> callback = null);

        void Update(IEnumerable<TAttr> entities);

        void Remove(IEnumerable<TAttr> entities, Action<TEntity> callback = null);

        IEnumerable<TEntity> Remove(IEnumerable<TID> ids, Action<TEntity> callback = null);
    }
}

