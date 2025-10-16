using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

public class StudentService : ICrudServiceAsync<Student>, IDisposable
{
    private readonly ConcurrentDictionary<Guid, Student> _storage = new();
    private readonly SemaphoreSlim _fileSemaphore = new(1, 1);
    private readonly string _filePath;

    public StudentService(string filePath = "students.json")
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        _ = LoadIfExistsAsync();
    }

    private async Task LoadIfExistsAsync()
    {
        if (!File.Exists(_filePath)) return;
        await _fileSemaphore.WaitAsync();
        try
        {
            using var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
            var items = await JsonSerializer.DeserializeAsync<List<Student>>(fs);
            if (items != null)
            {
                foreach (var s in items)
                    _storage[s.Id] = s;
            }
        }
        catch
        {

        }
        finally
        {
            _fileSemaphore.Release();
        }
    }

    public Task<bool> CreateAsync(Student element)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        if (element.Id == Guid.Empty) element.Id = Guid.NewGuid();
        var added = _storage.TryAdd(element.Id, element);
        return Task.FromResult(added);
    }

    public Task<Student> ReadAsync(Guid id)
    {
        _storage.TryGetValue(id, out var s);
        return Task.FromResult(s);
    }

    public Task<IEnumerable<Student>> ReadAllAsync()
    {
        var snapshot = _storage.Values.ToList();
        return Task.FromResult((IEnumerable<Student>)snapshot);
    }

    public Task<IEnumerable<Student>> ReadAllAsync(int page, int amount)
    {
        if (page < 1) page = 1;
        if (amount < 1) amount = 10;
        var skip = (page - 1) * amount;
        var pageItems = _storage.Values
            .OrderBy(s => s.Id)
            .Skip(skip)
            .Take(amount)
            .ToList();
        return Task.FromResult((IEnumerable<Student>)pageItems);
    }

    public Task<bool> UpdateAsync(Student element)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        if (!_storage.ContainsKey(element.Id)) return Task.FromResult(false);
        _storage[element.Id] = element;
        return Task.FromResult(true);
    }

    public Task<bool> RemoveAsync(Student element)
    {
        if (element == null) throw new ArgumentNullException(nameof(element));
        var removed = _storage.TryRemove(element.Id, out _);
        return Task.FromResult(removed);
    }

    public async Task<bool> SaveAsync()
    {
        var snapshot = _storage.Values.ToList();
        await _fileSemaphore.WaitAsync();
        try
        {
            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            using var fs = new FileStream(_filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true);
            await JsonSerializer.SerializeAsync(fs, snapshot, new JsonSerializerOptions { WriteIndented = true });
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

    public IEnumerator<Student> GetEnumerator()
    {
        var snapshot = _storage.Values.ToList();
        return snapshot.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Dispose()
    {
        _fileSemaphore?.Dispose();
    }
}
