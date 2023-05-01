namespace ChampionsOfTheRealm;

/// <summary>
/// Interface for objects which will be pooled and reused (Usually projectiles)
/// </summary>
public interface IPoolable
{
    void AddToScene();
    void RemoveFromScene();
}