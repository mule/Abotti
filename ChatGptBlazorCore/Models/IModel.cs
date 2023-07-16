namespace ChatGptBlazorCore.Models;

public interface IModel<T>
{
    public T Id { get; set; }
}