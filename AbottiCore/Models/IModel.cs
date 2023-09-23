namespace Abotti.Core.Models;

public interface IModel<T>
{
    public T Id { get; set; }
}