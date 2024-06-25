using System;
using System.Collections.Generic;
using System.Linq;

using Common.Core;
using Common.Repositories;

namespace Game.Core.Repositories
{
    public class Repository<TID, TObject, TAttribute> : IReadonlyRepository<TID, TObject, TAttribute>
        where TAttribute : IEntityAttributes<TObject>
    {
        protected readonly DictionaryRepository<TID, TObject, TAttribute> m_Repo = new ();
        
        public IEnumerable<TObject> Find(Func<TAttribute, bool> filter = null, 
            Func<IQueryable<TAttribute>, IOrderedQueryable<TAttribute>> orderBy = null)
        {
            return m_Repo.Find(filter, orderBy);
        }

        public TObject FindByID(TID id)
        {
            return m_Repo.FindByID(id);
        }

        public T FindByID<T>(TID id) where T : TObject
        {
            return m_Repo.FindByID<T>(id);
        }
    }
}
