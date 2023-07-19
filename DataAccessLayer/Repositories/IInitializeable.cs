using ChatGptBlazorCore.Models;

namespace DataAccessLayer.Repositories;

public interface IInitializeableRepository<TK, T> : IRepository<TK, T>
    where T : IModel<TK>
{
    public bool IsInitialized { get; }
    void Initialize(bool overwrite = false);
    void Initialize(IDictionary<TK, T> initialState, bool overwrite = false);

    Task InitializeAsync(bool overwrite = false);
    Task InitializeAsync(IDictionary<TK, T> initialState, bool overwrite = false);
}