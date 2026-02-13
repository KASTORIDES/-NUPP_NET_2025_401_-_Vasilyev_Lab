using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentSystem.Common
{
    public class CrudService<T> : ICrudService<T> where T : class
    {
        private readonly List<T> _storage = new();

        public void Create(T element)
        {
            _storage.Add(element);
        }

        public T Read(Guid id)
        {
            return _storage.FirstOrDefault(e =>
                (Guid)e.GetType().GetProperty("Id")!.GetValue(e)! == id)!;
        }

        public IEnumerable<T> ReadAll()
        {
            return _storage;
        }

        public void Update(T element)
        {
            var id = (Guid)element.GetType().GetProperty("Id")!.GetValue(element)!;
            var old = Read(id);
            if (old != null)
            {
                Remove(old);
                Create(element);
            }
        }

        public void Remove(T element)
        {
            _storage.Remove(element);
        }
    }
}
