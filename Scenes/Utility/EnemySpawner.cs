using Godot;

/// <summary>
/// Enemy spawner class to spawn enemies in a random position in the map. It is possible to set a custom start and end time,
/// set a delay between spawns and change the amount of enemies that are spawned.
/// </summary>
public partial class EnemySpawner : Node2D
{
    /// <summary>
    /// //Export array to store spawn point info
    /// </summary>
    /// <value>Array of SpawnInfo objects</value>
    [Export] public Godot.Collections.Array<SpawnInfo> Spawns { get; set; } 
    private int time; //variable to keep count of the time that has passed since spawning started

    private float limitLeft = 20f; //map coordinates limit left
    private float limitRight; //map coordinates limit right
    private float limitTop = 20f; //map coordinates limit top
    private float limitBottom; //map coordinates limit bottom

    /// <summary>
    /// Initialize method for initializing limit values
    /// </summary>
    /// <param name="vector">Vector with limit values</param>
    public void Initialize(Vector2 vector)
    {
        limitRight = vector.X;
        limitBottom = vector.Y;
    }

    /// <summary>
    /// Method to get a random position for the spawn point
    /// </summary>
    /// <returns>2D vector to reporesent the position of the spawn point</returns>
    private Vector2 GetRandomPosition()
    {
        float x = Globals.GetRandomFloat(limitLeft, limitRight);
        float y = Globals.GetRandomFloat(limitTop, limitBottom);
        GD.Print(limitTop);
        GD.Print(limitBottom);

        return new Vector2(x, y);
    }

    /// <summary>
    /// Method to spawn enemy on timer timeout
    /// </summary>
    private void OnTimerTimeout()
    {
        time++;
        foreach (var i in Spawns)
        {
            if (time >= i.TimeStart && time <= i.TimeEnd)
            {
                if (i.spawnDelayCounter < i.EnemySpawnDelay) i.spawnDelayCounter++;
                else
                {
                    i.spawnDelayCounter = 0;

                    PackedScene enemyScene = i.Enemy;
                    int counter = 0;
                    while (counter < i.EnemyNum)
                    {
                        var enemySpawn = enemyScene.Instantiate<Actor>();
                        AddChild(enemySpawn);
                        enemySpawn.GlobalPosition = GetRandomPosition();
                        counter++;
                    }
                }
            }
        }
    }
}
