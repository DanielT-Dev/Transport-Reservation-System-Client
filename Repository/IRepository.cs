using System.Collections.Generic;

namespace MPP_Client.Repository
{
    public interface IRepository<T, TId>
    {
        T             GetById(TId id);
        List<T>       GetAll();
        void          Add(T entity);
        void          Update(T entity);
        void          Delete(TId id);
    }
}