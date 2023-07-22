namespace ChatGptBlazorCore.Models;

public interface IRepository<in TK, T> where T : IModel<TK>
{
    (bool Ok, string[] Errors) Add(T item);
    (bool Ok, string[] Errors) Delete(TK key);
    (bool Ok, string[] Errors) Update(T item);
    (bool Ok, string[] Errors) DeleteAll();
    (bool Ok, T[] Result, string[] Errors) GetAll();
    (bool result, T? Result, string[]) Get(TK key);

    //Async methods
    Task<(bool Ok, string[] Errors)> AddAsync(T item);
    Task<(bool Ok, string[] Errors)> DeleteAsync(TK key);
    Task<(bool Ok, string[] Errors)> UpdateAsync(T item);
    Task<(bool Ok, string[] Errors)> DeleteAllAsync();
    Task<(bool Ok, T[] Result, string[] Errors)> GetAllAsync();
    Task<(bool Ok, T Result, string[] Errors)> GetAsync(TK key);
}