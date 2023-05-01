namespace ChampionsOfTheRealm;

using System.Collections.Concurrent;

/// <summary>
/// Object pool increases performance by storing and reusing objects
/// in this way cost to add new objects to the scene is dramatically reduced
/// </summary>
/// <typeparam name="T">Any object with IPoolable interface</typeparam>
public partial class ObjectPool<T> where T : IPoolable, new()
{
    private readonly ConcurrentBag<T> items; // items.
    private int counter = 0; // items currently in the pool.
    private int max; // maximum amount of items in a pool.

    /// <summary>
    /// Method for creating object pool and setting the max amount of items in a pool.
    /// </summary>
    /// <param name="max">Pool size</param>
    public ObjectPool(int max = 100)
    {
        this.items = new ConcurrentBag<T>();
        this.max = max;
    }

    /// <summary>
    /// Method for returning objects to the pool.
    /// </summary>
    /// <param name="item">item to be returned.</param>
    public void Release(T item)
    {
        if (counter < max)
        {
            items.Add(item);
            counter++;
        }
    }

    /// <summary>
    /// Method for getting an item from the pool..
    /// </summary>
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