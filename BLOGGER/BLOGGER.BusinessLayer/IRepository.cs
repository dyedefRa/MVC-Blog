using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLOGGER.BusinessLayer
{
  public  interface IRepository<T> where T:class
    {

        T GetByID(int id);
        List<T> Select();
        bool Delete(int id);
        bool Update(T Entity);
        bool Insert(T Entity);
        bool SAVE();
    }
}
