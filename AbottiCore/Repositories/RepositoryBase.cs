using Abotti.Core.Models;

namespace Abotti.Core.Repositories;

public abstract class RepositoryBase<TK, T> : IRepository<TK, T> where T : IModel<TK> where TK : notnull
{
    protected IDictionary<TK, T> items;

    public RepositoryBase()
    {
        items = new Dictionary<TK, T>();
    }

    public RepositoryBase(IDictionary<TK, T> initialState)
    {
        items = initialState;
    }

    public virtual (bool Ok, string[] Errors) Add(T item)
    {
        var result = items.TryAdd(item.Id, item);
        return (result, result ? Array.Empty<string>() : new[] { "Item already exists" });
    }

    public virtual (bool Ok, string[] Errors) Delete(TK key)
    {
        var result = items.Remove(key);
        return (result, result ? Array.Empty<string>() : new[] { "Item not found" });
    }

    public virtual (bool Ok, string[] Errors) Update(T item)
    {
        if (items.ContainsKey(item.Id))
            items[item.Id] = item;
        else
            items.Add(item.Id, item);

        return (true, Array.Empty<string>());
    }

    public virtual (bool Ok, string[] Errors) DeleteAll()
    {
        items.Clear();
        return (true, Array.Empty<string>());
    }

    public virtual (bool Ok, T[] Result, string[] Errors) GetAll()
    {
        return (true, items.Values.ToArray(), Array.Empty<string>());
    }

    public virtual (bool result, T? Result, string[]) Get(TK key)
    {
        var result = items.TryGetValue(key, out var item);
        return (result, item, result ? Array.Empty<string>() : new[] { "Item not found" });
    }

    public virtual async Task<(bool Ok, string[] Errors)> AddAsync(T item)
    {
        var result = await Task.FromResult(Add(item));
        return result;
    }

    public virtual async Task<(bool Ok, string[] Errors)> DeleteAsync(TK key)
    {
        var result = await Task.FromResult(Delete(key));
        return result;
    }

    public virtual async Task<(bool Ok, string[] Errors)> UpdateAsync(T item)
    {
        var result = await Task.FromResult(Update(item));
        return result;
    }

    public virtual async Task<(bool Ok, string[] Errors)> DeleteAllAsync()
    {
        var result = await Task.FromResult(DeleteAll());
        return result;
    }

    public virtual async Task<(bool Ok, T[] Result, string[] Errors)> GetAllAsync()
    {
        var result = await Task.FromResult(GetAll());
        return result;
    }

    public virtual async Task<(bool Ok, T? Result, string[] Errors)> GetAsync(TK key)
    {
        var result = await Task.FromResult(Get(key));
        return result;
    }
}