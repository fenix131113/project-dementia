using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace Utils.ResourcesLoading
{
    public class DataRepository<T> where T : Object
    {
        private readonly Dictionary<Type, List<T>> _repository = new();

        public void Create(Type key, IEnumerable<T> res)
        {
            _repository.TryAdd(key, new List<T>());
            
            foreach (var r in res)
                _repository[key].Add(r);
        }

        public void Delete(Type key, T item)
        {
            if (_repository.TryGetValue(key, out var value))
                value.Remove(item);
        }

        public List<TR> GetItem<TR>() where TR : class
        {
            return (from so in _repository[typeof(TR)] select so as TR).ToList();
        }

        public int GetCount()
        {
            return _repository.Count;
        }
    }
}