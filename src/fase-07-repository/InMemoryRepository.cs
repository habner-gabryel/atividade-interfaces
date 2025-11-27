using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Fase07Repository
{
    // contrato simples para entidades com Id
    public interface IEntity { Guid Id { get; set; } }

    // contrato do repositório
    public interface IRepository<T> where T : IEntity
    {
        T Add(T entity);
        T GetById(Guid id);
        IEnumerable<T> List();
        IEnumerable<T> Find(Func<T, bool> predicate);
        bool Update(T entity);
        bool Delete(Guid id);
    }

    // implementação in-memory thread-safe
    public class InMemoryRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly ConcurrentDictionary<Guid, T> _store = new ConcurrentDictionary<Guid, T>();

        public T Add(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (entity.Id == Guid.Empty) entity.Id = Guid.NewGuid();
            _store[entity.Id] = entity;
            return entity;
        }

        public T GetById(Guid id)
        {
            _store.TryGetValue(id, out var value);
            return value;
        }

        public IEnumerable<T> List() => _store.Values.ToList();

        public IEnumerable<T> Find(Func<T, bool> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));
            return _store.Values.Where(predicate).ToList();
        }

        public bool Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (entity.Id == Guid.Empty) return false;
            return _store.AddOrUpdate(entity.Id, entity, (k, old) => entity) != null;
        }

        public bool Delete(Guid id)
        {
            return _store.TryRemove(id, out _);
        }
    }

    // Exemplo de entidade: Movie
    public class Movie : IEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
    }

    // Serviço que usa o repositório sem conhecer detalhes internos
    public class MovieService
    {
        private readonly IRepository<Movie> _repo;

        public MovieService(IRepository<Movie> repo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }

        public Movie Create(string title, int year)
        {
            var m = new Movie { Title = title, Year = year };
            return _repo.Add(m);
        }

        public Movie Get(Guid id) => _repo.GetById(id);
        public IEnumerable<Movie> GetAll() => _repo.List();
        public IEnumerable<Movie> SearchByTitle(string term) => _repo.Find(m => m.Title?.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);
        public bool Update(Movie movie) => _repo.Update(movie);
        public bool Delete(Guid id) => _repo.Delete(id);
    }

    // Demo program
    public static class RepositoryDemo
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=== Fase 7: Repository Progressivo (InMemory) ===\n");

            var repo = new InMemoryRepository<Movie>();
            var service = new MovieService(repo);

            var m1 = service.Create("The Matrix", 1999);
            var m2 = service.Create("Interstellar", 2014);

            Console.WriteLine("All movies:");
            foreach (var mv in service.GetAll())
                Console.WriteLine($"- {mv.Id} | {mv.Title} ({mv.Year})");

            Console.WriteLine("\nSearch 'inter':");
            foreach (var mv in service.SearchByTitle("inter"))
                Console.WriteLine($"- {mv.Title}");

            Console.WriteLine("\nUpdate movie title:");
            m2.Title = "Interstellar (updated)";
            service.Update(m2);
            Console.WriteLine(service.Get(m2.Id).Title);

            Console.WriteLine("\nDelete first movie and list again:");
            service.Delete(m1.Id);
            foreach (var mv in service.GetAll())
                Console.WriteLine($"- {mv.Title}");

            Console.WriteLine("\nNote: MovieService never knows repo is InMemoryRepository<T>.");
        }
    }
}
