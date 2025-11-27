using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

// Simples implementação CSV que respeita um contrato IRepository<T>
// Esta classe assume que já existe uma interface IRepository<T> e IEntity (definidas em fase-07).
public class CsvRepository<T> : IRepository<T> where T : IEntity
{
    private readonly string _filePath;
    private readonly Func<T, string> _serializer;       // T -> CSV line
    private readonly Func<string, T> _deserializer;     // CSV line -> T
    private readonly object _lock = new object();
    private readonly List<T> _items = new List<T>();

    public CsvRepository(string filePath, Func<T,string> serializer, Func<string,T> deserializer)
    {
        _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));

        LoadFromFile();
    }

    private void LoadFromFile()
    {
        lock (_lock)
        {
            _items.Clear();
            if (!File.Exists(_filePath))
                return; // arquivo inexistente => repositório vazio

            foreach (var line in File.ReadLines(_filePath, Encoding.UTF8))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var entity = _deserializer(line);
                if (entity != null)
                    _items.Add(entity);
            }
        }
    }

    private void PersistToFile()
    {
        var temp = _filePath + ".tmp";
        lock (_lock)
        {
            var lines = _items.Select(i => _serializer(i)).ToArray();
            File.WriteAllLines(temp, lines, Encoding.UTF8);
            // Replace atomically when possible
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

// Exemplo de uso rápido (opcional): serializador/deserializador para um tipo Movie
// public class Movie : IEntity { public Guid Id { get; set; } public string Title { get; set; } public int Year { get; set; } }
// var csv = new CsvRepository<Movie>("movies.csv", m => $"{m.Id}|{Escape(m.Title)}|{m.Year}", line => Parse(line));
