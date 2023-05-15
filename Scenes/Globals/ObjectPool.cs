namespace ChampionsOfTheRealm;

using System.Collections.Concurrent;

/// <summary>
/// Object pool increases performance by storing and reusing objects
/// in this way cost to add new objects to the scene is dramatically reduced
/// </summary>
/// <typeparam name="T">Any object with IPoolable interface</typeparam>
public partial class ObjectPool<T> where T : IPoolable, new()
{
    /// <summary>
    /// Colletion of items in the object pool
    /// </summary>
    private readonly ConcurrentBag<T> items;

    /// <summary>
    /// Number of items currently in the pool
    /// </summary>
    private int counter = 0;

    /// <summary>
    /// Maximum amount of items in the pool
    /// </summary>
    private int max;

    /// <summary>
    /// Method for creating object pool
    ///  and setting the max amount of items in a pool.
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
    /// Method for retrieving object from the pool.
    /// If an item is available in the pool, it is returned.
    /// Otherwise, a new item is created and added to the pool
    /// </summary>
    /// <returns>Returns an object of type T</returns>
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