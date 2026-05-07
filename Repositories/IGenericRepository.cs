namespace ProjectManagement.Repositories
{
    public interface IGenericRepository <T>
    {
        public IEnumerable<T> GetAll();
        public T GetById(int id);
        public T GetById(string id);
        public void Add(T entity);
        public void Update(T entity);
        public void Delete(T entity);
        public void Save();
    }
}
