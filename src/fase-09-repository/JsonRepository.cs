using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

// Implementação JsonRepository<T> que respeita contrato IRepository<T>
// Persiste entidades em um arquivo JSON.
public class JsonRepository<T> : IRepository<T> where T : IEntity
{
    private readonly string _filePath;
    private readonly object _lock = new object();
    private readonly List<T> _items = new List<T>();
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonRepository(string filePath)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };
        LoadFromFile();
    }

    private void LoadFromFile()
    {
        lock (_lock)
        {
            _items.Clear();
            if (!File.Exists(_filePath))
                return; // arquivo inexistente => repositório vazio

            try
            {
                var json = File.ReadAllText(_filePath, Encoding.UTF8);
                if (string.IsNullOrWhiteSpace(json))
                    return;

                var items = JsonSerializer.Deserialize<List<T>>(json, _jsonOptions);
                if (items != null)
                    _items.AddRange(items);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Erro ao desserializar JSON: {ex.Message}", ex);
            }
        }
    }

    private void PersistToFile()
    {
        var temp = _filePath + ".tmp";
        lock (_lock)
        {
            var json = JsonSerializer.Serialize(_items, _jsonOptions);
            File.WriteAllText(temp, json, Encoding.UTF8);
            // Replace atomicamente quando possível
            File.Copy(temp, _filePath, true);
            File.Delete(temp);
        }
    }

    public void Add(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        lock (_lock)
        {
            _items.Add(entity);
            PersistToFile();
        }
    }

    public T GetById(Guid id)
    {
        lock (_lock)
        {
            return _items.FirstOrDefault(x => x.Id == id);
        }
    }

    public IEnumerable<T> List()
    {
        lock (_lock)
        {
            return _items.ToList(); // snapshot
        }
    }

    public IEnumerable<T> Find(Func<T, bool> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        lock (_lock)
        {
            return _items.Where(predicate).ToList();
        }
    }

    public void Update(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        lock (_lock)
        {
            var idx = _items.FindIndex(x => x.Id == entity.Id);
            if (idx == -1) throw new InvalidOperationException("Entity not found");
            _items[idx] = entity;
            PersistToFile();
        }
    }

    public void Delete(Guid id)
    {
        lock (_lock)
        {
            var removed = _items.RemoveAll(x => x.Id == id) > 0;
            if (removed)
                PersistToFile();
        }
    }
}

// Exemplo de Factory para centralizar a decisão de qual repositório usar:
// public class RepositoryFactory
// {
//     public static IRepository<T> CreateRepository<T>(string format, string filePath = null) where T : IEntity
//     {
//         return format.ToLowerInvariant() switch
//         {
//             "json" => new JsonRepository<T>(filePath ?? $"{typeof(T).Name}.json"),
//             "csv" => new CsvRepository<T>(filePath ?? $"{typeof(T).Name}.csv", /* serializer */, /* deserializer */),
//             "memory" => new InMemoryRepository<T>(),
//             _ => throw new ArgumentException($"Formato desconhecido: {format}")
//         };
//     }
// }
