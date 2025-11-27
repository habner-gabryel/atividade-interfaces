using System;
using System.Collections.Generic;
using System.Linq;

// Contrato e implementação (reutilizados das fases anteriores)
public interface IEntity
{
    Guid Id { get; set; }
}

public interface IRepository<T> where T : IEntity
{
    void Add(T entity);
    T GetById(Guid id);
    IEnumerable<T> List();
    IEnumerable<T> Find(Func<T, bool> predicate);
    void Update(T entity);
    void Delete(Guid id);
}

public class Movie : IEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
}

// Implementação fake para testes (seam para testabilidade)
public class InMemoryRepository<T> : IRepository<T> where T : IEntity
{
    private readonly List<T> _items = new List<T>();

    public void Add(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        _items.Add(entity);
    }

    public T GetById(Guid id)
    {
        return _items.FirstOrDefault(x => x.Id == id);
    }

    public IEnumerable<T> List()
    {
        return _items.ToList();
    }

    public IEnumerable<T> Find(Func<T, bool> predicate)
    {
        return _items.Where(predicate).ToList();
    }

    public void Update(T entity)
    {
        var idx = _items.FindIndex(x => x.Id == entity.Id);
        if (idx == -1) throw new InvalidOperationException("Entity not found");
        _items[idx] = entity;
    }

    public void Delete(Guid id)
    {
        _items.RemoveAll(x => x.Id == id);
    }
}

// Serviço a ser testado (depende de IRepository<T>, não de implementação concreta)
public class MovieService
{
    private readonly IRepository<Movie> _repo;

    public MovieService(IRepository<Movie> repo)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public void AddMovie(string title, int year)
    {
        var movie = new Movie
        {
            Id = Guid.NewGuid(),
            Title = title,
            Year = year
        };
        _repo.Add(movie);
    }

    public Movie GetMovieById(Guid id)
    {
        return _repo.GetById(id);
    }

    public IEnumerable<Movie> ListAllMovies()
    {
        return _repo.List();
    }

    public IEnumerable<Movie> FindMoviesByYear(int year)
    {
        return _repo.Find(m => m.Year == year);
    }

    public void UpdateMovieTitle(Guid id, string newTitle)
    {
        var movie = _repo.GetById(id);
        if (movie == null)
            throw new InvalidOperationException($"Filme com Id {id} não encontrado");
        
        movie.Title = newTitle;
        _repo.Update(movie);
    }

    public void DeleteMovie(Guid id)
    {
        _repo.Delete(id);
    }

    public void PrintAllMovies()
    {
        var movies = ListAllMovies();
        foreach (var movie in movies)
        {
            Console.WriteLine($"[{movie.Id}] {movie.Title} ({movie.Year})");
        }
    }
}

// ========== EXEMPLOS DE TESTES (sem framework, simulando AAA pattern) ==========

public class MovieServiceTests
{
    private MovieService _service;
    private IRepository<Movie> _repo;

    private void Setup()
    {
        // Arrange: criar repositório fake (seam por DI)
        _repo = new InMemoryRepository<Movie>();
        _service = new MovieService(_repo);
    }

    // Cenário 1: Adicionar e recuperar filme
    public void Test_AddMovie_ShouldBeRetrievable()
    {
        Setup();
        
        // Act
        _service.AddMovie("Inception", 2010);
        var movies = _service.ListAllMovies().ToList();
        var firstMovie = movies.FirstOrDefault();

        // Assert (comportamento observável)
        if (movies.Count != 1)
            Console.WriteLine("❌ FALHA: Esperado 1 filme, mas encontrado " + movies.Count);
        else if (firstMovie.Title != "Inception")
            Console.WriteLine("❌ FALHA: Esperado título 'Inception', mas recebido '" + firstMovie.Title + "'");
        else if (firstMovie.Year != 2010)
            Console.WriteLine("❌ FALHA: Esperado ano 2010, mas recebido " + firstMovie.Year);
        else
            Console.WriteLine("✅ SUCESSO: Filme adicionado e recuperado corretamente");
    }

    // Cenário 2: Listar múltiplos filmes
    public void Test_ListAllMovies_ShouldReturnAllAdded()
    {
        Setup();
        
        // Act
        _service.AddMovie("Inception", 2010);
        _service.AddMovie("Interstellar", 2014);
        _service.AddMovie("Tenet", 2020);
        var allMovies = _service.ListAllMovies().ToList();

        // Assert
        if (allMovies.Count != 3)
            Console.WriteLine("❌ FALHA: Esperado 3 filmes, mas encontrado " + allMovies.Count);
        else
            Console.WriteLine("✅ SUCESSO: Listagem retornou exatamente 3 filmes");
    }

    // Cenário 3: Atualizar filme
    public void Test_UpdateMovieTitle_ShouldChangeTitle()
    {
        Setup();
        
        // Act
        _service.AddMovie("Inception", 2010);
        var movie = _service.ListAllMovies().First();
        var movieId = movie.Id;
        
        _service.UpdateMovieTitle(movieId, "Inception (Revised)");
        var updated = _service.GetMovieById(movieId);

        // Assert
        if (updated.Title != "Inception (Revised)")
            Console.WriteLine("❌ FALHA: Título não foi atualizado. Recebido: " + updated.Title);
        else
            Console.WriteLine("✅ SUCESSO: Título atualizado corretamente");
    }

    // Cenário 4: Atualizar filme inexistente (erro esperado)
    public void Test_UpdateNonExistentMovie_ShouldThrow()
    {
        Setup();
        
        // Act & Assert
        try
        {
            _service.UpdateMovieTitle(Guid.NewGuid(), "New Title");
            Console.WriteLine("❌ FALHA: Deveria ter lançado InvalidOperationException");
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("não encontrado"))
                Console.WriteLine("✅ SUCESSO: Exceção lançada corretamente com mensagem apropriada");
            else
                Console.WriteLine("❌ FALHA: Exceção lançada mas mensagem inadequada: " + ex.Message);
        }
    }

    // Cenário 5: Buscar filmes por critério
    public void Test_FindMoviesByYear_ShouldFilterCorrectly()
    {
        Setup();
        
        // Act
        _service.AddMovie("Inception", 2010);
        _service.AddMovie("Interstellar", 2014);
        _service.AddMovie("Tenet", 2020);
        var movies2010s = _service.FindMoviesByYear(2010).ToList();

        // Assert
        if (movies2010s.Count != 1)
            Console.WriteLine("❌ FALHA: Esperado 1 filme de 2010, mas encontrado " + movies2010s.Count);
        else if (movies2010s.First().Title != "Inception")
            Console.WriteLine("❌ FALHA: Esperado 'Inception' em 2010");
        else
            Console.WriteLine("✅ SUCESSO: Filtro por ano funcionou corretamente");
    }

    // Cenário 6: Deletar filme
    public void Test_DeleteMovie_ShouldRemove()
    {
        Setup();
        
        // Act
        _service.AddMovie("Inception", 2010);
        var movieId = _service.ListAllMovies().First().Id;
        
        _service.DeleteMovie(movieId);
        var remaining = _service.ListAllMovies().ToList();

        // Assert
        if (remaining.Count != 0)
            Console.WriteLine("❌ FALHA: Após deletar, esperado 0 filmes, mas encontrado " + remaining.Count);
        else
            Console.WriteLine("✅ SUCESSO: Filme deletado corretamente");
    }

    // Cenário 7: Comportamento com repositório vazio
    public void Test_GetMovieFromEmptyRepo_ShouldReturnNull()
    {
        Setup();
        
        // Act
        var result = _service.GetMovieById(Guid.NewGuid());

        // Assert
        if (result != null)
            Console.WriteLine("❌ FALHA: Esperado null ao buscar em repo vazio");
        else
            Console.WriteLine("✅ SUCESSO: Retorna null corretamente para repo vazio");
    }
}

// ========== EXECUÇÃO DOS TESTES ==========

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Fase 10 — Testabilidade: Dublês e Costuras ===\n");
        
        var tests = new MovieServiceTests();

        Console.WriteLine("Cenário 1: Adicionar e recuperar filme");
        tests.Test_AddMovie_ShouldBeRetrievable();

        Console.WriteLine("\nCenário 2: Listar múltiplos filmes");
        tests.Test_ListAllMovies_ShouldReturnAllAdded();

        Console.WriteLine("\nCenário 3: Atualizar filme");
        tests.Test_UpdateMovieTitle_ShouldChangeTitle();

        Console.WriteLine("\nCenário 4: Atualizar filme inexistente");
        tests.Test_UpdateNonExistentMovie_ShouldThrow();

        Console.WriteLine("\nCenário 5: Buscar filmes por critério");
        tests.Test_FindMoviesByYear_ShouldFilterCorrectly();

        Console.WriteLine("\nCenário 6: Deletar filme");
        tests.Test_DeleteMovie_ShouldRemove();

        Console.WriteLine("\nCenário 7: Buscar em repositório vazio");
        tests.Test_GetMovieFromEmptyRepo_ShouldReturnNull();

        Console.WriteLine("\n=== Todos os testes executados ===");
    }
}
