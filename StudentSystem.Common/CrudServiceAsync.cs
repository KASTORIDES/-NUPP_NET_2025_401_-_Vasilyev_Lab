using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using StudentSystem.Infrastructure.Repositories;

namespace StudentSystem.Common
{
    public class CrudServiceAsync<T> : ICrudServiceAsync<T> where T : class
    {
        private readonly IRepository<T> _repository;

        public CrudServiceAsync(IRepository<T> repository)
        {   
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        // Восстановлен конструктор для старого сценария: new CrudServiceAsync<T>(storageFile)
        public CrudServiceAsync(string storageFile)
        {
            if (string.IsNullOrWhiteSpace(storageFile)) throw new ArgumentException("storageFile must not be empty", nameof(storageFile));
            _repository = new FileRepository(storageFile);
        }

        public async Task<bool> CreateAsync(T element)
        {
            if (element == null) return false;
            await _repository.AddAsync(element);
            return true;
        }

        public async Task<T?> ReadAsync(Guid id)
        {
            var all = await _repository.GetAllAsync();
            foreach (var item in all)
            {
                var prop = item.GetType().GetProperty("Guid");
                if (prop != null)
                {
                    var value = prop.GetValue(item);
                    if (value is Guid g && g == id)
                        return item;
                }
            }
            return null;
        }

        public async Task<IEnumerable<T>> ReadAllAsync() =>
            await _repository.GetAllAsync();

        public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
        {
            var all = await _repository.GetAllAsync();
            return all.Skip((page - 1) * amount).Take(amount);
        }

        public async Task<bool> UpdateAsync(T element)
        {
            if (element == null) return false;
            await _repository.Update(element);
            return true;
        }

        public async Task<bool> RemoveAsync(T element)
        {
            if (element == null) return false;
            await _repository.Delete(element);
            return true;
        }

        public async Task<bool> SaveAsync()
        {
            // Если репозиторий — файловый, принудительно сохранить на диск
            if (_repository is FileRepository fileRepo)
            {
                await fileRepo.PersistAsync();
                return true;
            }

            // Для EF-репозиториев — ничего не делать (они сами сохраняют)
            return await Task.FromResult(true);
        }

        // IEnumerable<T> реализация (используется интерфейс ICrudServiceAsync<T>)
        public IEnumerator<T> GetEnumerator()
        {
            var all = _repository.GetAllAsync().GetAwaiter().GetResult();
            return all.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        // Встроенная файловая реализация IRepository<T> — не меняет внешнего контракта
        private sealed class FileRepository : IRepository<T>
        {
            private readonly string _filePath;
            private List<T> _items;
            private readonly object _sync = new();

            public FileRepository(string filePath)
            {
                _filePath = Path.GetFullPath(filePath);
                _items = new List<T>();

                if (File.Exists(_filePath))
                {
                    try
                    {
                        var text = File.ReadAllText(_filePath);
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            var list = JsonSerializer.Deserialize<List<T>>(text, opts);
                            if (list != null) _items = list;
                        }
                    }
                    catch
                    {
                        _items = new List<T>();
                    }
                }
            }

            public Task<T> GetByIdAsync(int id)
            {
                lock (_sync)
                {
                    var found = _items.FirstOrDefault(item =>
                    {
                        var prop = item.GetType().GetProperty("Id");
                        if (prop != null && prop.PropertyType == typeof(int))
                        {
                            var value = prop.GetValue(item);
                            return value is int iv && iv == id;
                        }
                        return false;
                    });
                    return Task.FromResult(found!);
                }
            }

            public Task<IEnumerable<T>> GetAllAsync()
            {
                lock (_sync)
                {
                    return Task.FromResult<IEnumerable<T>>(_items.ToList());
                }
            }

            public Task AddAsync(T entity)
            {
                lock (_sync) { _items.Add(entity); }
                return Task.CompletedTask;
            }

            public Task Update(T entity)
            {
                lock (_sync)
                {
                    var idProp = entity.GetType().GetProperty("Id");
                    bool replaced = false;
                    if (idProp != null && idProp.PropertyType == typeof(int))
                    {
                        var idVal = idProp.GetValue(entity);
                        if (idVal is int id)
                        {
                            for (int i = 0; i < _items.Count; i++)
                            {
                                var existingIdProp = _items[i].GetType().GetProperty("Id");
                                if (existingIdProp != null && existingIdProp.PropertyType == typeof(int))
                                {
                                    var existingVal = existingIdProp.GetValue(_items[i]);
                                    if (existingVal is int existingId && existingId == id)
                                    {
                                        _items[i] = entity;
                                        replaced = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    if (!replaced)
                    {
                        var idx = _items.IndexOf(entity);
                        if (idx >= 0) _items[idx] = entity;
                    }
                }
                return Task.CompletedTask;
            }

            public Task Delete(T entity)
            {
                lock (_sync)
                {
                    if (!_items.Remove(entity))
                    {
                        var idProp = entity.GetType().GetProperty("Id");
                        if (idProp != null && idProp.PropertyType == typeof(int))
                        {
                            var val = idProp.GetValue(entity);
                            if (val is int id)
                            {
                                _items.RemoveAll(item =>
                                {
                                    var existingIdProp = item.GetType().GetProperty("Id");
                                    return existingIdProp != null
                                        && existingIdProp.PropertyType == typeof(int)
                                        && existingIdProp.GetValue(item) is int existingId
                                        && existingId == id;
                                });
                            }
                        }
                    }
                }
                return Task.CompletedTask;
            }

            public async Task DeleteAllAsync()
            {
                lock (_sync) { _items.Clear(); }
                await PersistAsync();
            }

            public async Task PersistAsync()
            {
                List<T> snapshot;
                lock (_sync) { snapshot = _items.ToList(); }

                var opts = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(snapshot, opts);

                var dir = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

                await File.WriteAllTextAsync(_filePath, json);
            }
        }
    }
}
