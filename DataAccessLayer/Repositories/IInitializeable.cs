using Abotti.Core.Models;
using Abotti.Core.Repositories;

namespace Abotti.DataAccessLayer.Repositories;

public interface IInitializeableRepository<TK, T> : IRepository<TK, T>
    where T : IModel<TK>
{
    public bool IsInitialized { get; }
    void Initialize(bool overwrite = false);
    void Initialize(IDictionary<TK, T> initialState, bool overwrite = false);

    Task InitializeAsync(bool overwrite = false);
    Task InitializeAsync(IDictionary<TK, T> initialState, bool overwrite = false);
}