namespace Common.Infrastructure.RabbitMq.Models;

public sealed class Batch<T>
{
    private readonly List<T> _items = [];
    public IEnumerable<T> Items => _items;

    private void Add(T item)
    {
        _items.Add(item);
    }
}