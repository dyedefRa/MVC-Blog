using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLOGGER.Data;

namespace BLOGGER.BusinessLayer
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly BloggerContext ctx = new BloggerContext();
        private DbSet<T> _objectSet;


        public Repository()
        {
            _objectSet = ctx.Set<T>();
        }


        public bool Delete(int id)
        {
            _objectSet.Remove(GetByID(id));
            return SAVE();
        }

        public T GetByID(int id)
        {
            return _objectSet.Find(id);
        }

        public bool Insert(T Entity)
        {
            _objectSet.Add(Entity);
           
            return SAVE();
        }

        public bool SAVE()
        {
            return ctx.SaveChanges() > 0;
        }

        public List<T> Select()
        {
        return    _objectSet.ToList();
        }

        public bool Update(T Entity)
        {
            return SAVE();
        }
    }
}
