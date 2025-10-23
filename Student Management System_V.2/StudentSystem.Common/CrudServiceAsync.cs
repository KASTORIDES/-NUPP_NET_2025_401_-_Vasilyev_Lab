using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace StudentSystem.Common
{
    public class CrudServiceAsync<T> : ICrudServiceAsync<T> where T : class
    {
        // Використовуємо ConcurrentDictionary для thread-safe зберігання
        private readonly ConcurrentDictionary<Guid, T> _storage = new();
        public string FilePath { get; }

        // SemaphoreSlim для контролю доступу до файлу при збереженні
        private readonly SemaphoreSlim _fileSemaphore = new(1, 1);

        // Lock-приклад (внутрішній об'єкт)
        private readonly object _updateLock = new();

        public CrudServiceAsync(string filePath)
        {
            FilePath = filePath;
        }

        public async Task<bool> CreateAsync(T element)
        {
            if (element == null) return false;
            var id = (Guid?)element.GetType().GetProperty("Id")?.GetValue(element);
            if (id == null || id == Guid.Empty)
            {
                // якщо немає Id або порожній - пробуємо встановити новий
                var prop = element.GetType().GetProperty("Id");
                if (prop != null && prop.CanWrite)
                    prop.SetValue(element, Guid.NewGuid());
                id = (Guid?)prop?.GetValue(element);
            }

            if (id == null) return false;
            return _storage.TryAdd(id.Value, element);
        }

        public Task<T?> ReadAsync(Guid id)
        {
            _storage.TryGetValue(id, out var val);
            return Task.FromResult<T?>(val);
        }

        public Task<IEnumerable<T>> ReadAllAsync()
        {
            return Task.FromResult<IEnumerable<T>>(_storage.Values.ToList());
        }

        public Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
        {
            if (page < 1) page = 1;
            if (amount < 1) amount = 10;
            var items = _storage.Values
                .Skip((page - 1) * amount)
                .Take(amount)
                .ToList();
            return Task.FromResult<IEnumerable<T>>(items);
        }

        public Task<bool> UpdateAsync(T element)
        {
            if (element == null) return Task.FromResult(false);
            var id = (Guid?)element.GetType().GetProperty("Id")?.GetValue(element);
            if (id == null) return Task.FromResult(false);

            // lock — приклад використання block lock при оновленні
            lock (_updateLock)
            {
                if (!_storage.ContainsKey(id.Value)) return Task.FromResult(false);
                _storage[id.Value] = element;
            }
            return Task.FromResult(true);
        }

        public Task<bool> RemoveAsync(T element)
        {
            if (element == null) return Task.FromResult(false);
            var id = (Guid?)element.GetType().GetProperty("Id")?.GetValue(element);
            if (id == null) return Task.FromResult(false);

            return Task.FromResult(_storage.TryRemove(id.Value, out _));
        }

        // Асинхронне збереження колекції у файл (JSON)
        public async Task<bool> SaveAsync()
        {
            await _fileSemaphore.WaitAsync();
            try
            {
                var list = _storage.Values.ToList();
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNameCaseInsensitive = true
                };
                using FileStream fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
                await JsonSerializer.SerializeAsync(fs, list, options);
                await fs.FlushAsync();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                _fileSemaphore.Release();
            }
        }

        // IEnumerable<T> реалізація
        public IEnumerator<T> GetEnumerator() => _storage.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
