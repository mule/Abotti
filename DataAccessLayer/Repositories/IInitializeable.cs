using ChatGptBlazorCore.Models;

namespace DataAccessLayer.Repositories;

public interface IInitializeableRepository<TK, T> : IRepository<TK, T>
    where T : IModel<TK>
{
    public bool IsInitialized { get; }
    void Initialize();
    void Initialize(IDictionary<TK, T> initialState);

    Task InitializeAsync();
    Task InitializeAsync(IDictionary<TK, T> initialState);
}