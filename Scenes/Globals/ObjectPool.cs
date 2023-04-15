using System.Collections.Concurrent;

/// <summary>
/// Object pool increases performance by storing and reusing objects
/// in this way cost to add new objects to the scene is dramatically reduced
/// </summary>
/// <typeparam name="T">Any object with IPoolable interface</typeparam>
public partial class ObjectPool<T> where T : IPoolable, new()
{
    private readonly ConcurrentBag<T> items;
    private int counter = 0;
    private int max;

    public ObjectPool(int max = 100)
    {
        this.items = new ConcurrentBag<T>();
        this.max = max;
    }

    public void Release(T item)
    {
        if (counter < max)
        {
            items.Add(item);
            counter++;
        }
    }
    public T Get()
    {
        T item;
        if (items.TryTake(out item))
        {
            counter--;
            return item;
        }
        else
        {
            T obj = new T();
            items.Add(obj);
            counter++;
            return obj;
        }

    }
}