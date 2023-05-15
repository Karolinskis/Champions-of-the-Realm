namespace ChampionsOfTheRealm;

/// <summary>
/// Enemy spawner class responsible for spawning enemies in random positions on the map.
/// It allows customization of start and end times, spawn delay, and the number enemies to be spawned.
/// </summary>
public partial class EnemySpawner : Node2D
{
    /// <summary>
    /// Array of eneemy scenes to be spawned.
    /// </summary>
    /// <value>An array of PackedScene objects representing enemy scenes</value>
    [Export] public Godot.Collections.Array<PackedScene> Enemies { get; set; }

    private Timer timer;
    private Player target;

    private int time; // Variable to keep track of the time that has passed since spawning started

	/// <summary>
	/// Export variable to change how far from the player enemies should spawn
	/// </summary>
	[Export] public int enemySpawnDistanceLimit = 400;

    private float limitLeft = 20f; // Map coordinates limit left
    private float limitRight; // Map coordinates limit right
    private float limitTop = 20f; // Map coordinates limit top
    private float limitBottom; // Map coordinates limit bottom

    public override void _Ready()
    {
        base._Ready();
        timer = GetNode<Timer>("Timer");
    }

    /// <summary>
    /// Initializes the enemy spawner with the map limit values and targer player.
    /// </summary>
    /// <param name="vector">Vector containing the limit values of the map</param>
    /// <param name="player">The player object that enemies target</param>
    public void Initialize(Vector2 vector, Player player)
    {
        limitRight = vector.X; //Setting right limit of the map
        limitBottom = vector.Y; //Setting bottom limit of the map

        target = player; //Setting target object for the enemies

        timer.Start(); //Starting the timer
    }

    /// <summary>
    /// Gets a random enemy spawn position within the bounds of the map
    /// </summary>
    /// <returns>A Vector2 object representing the position coordinates</returns>
    private Vector2 GetEnemSpawnPosition()
    {
        Vector2 position = Globals.GetRandomPositionWithinRadius(target.Position, enemySpawnDistanceLimit);

        position.X = Math.Clamp(position.X, limitLeft, limitRight);
        position.Y = Math.Clamp(position.Y, limitTop, limitBottom);

        return position;
    }

    /// <summary>
    /// Spawns an enemy when the timer times out
    /// </summary>
    private void OnTimerTimeout()
    {
        time++;
        if (time >= 10 && timer.WaitTime > 0.2) //Every 10 seconds the timer wait time is decreased so that enemies spawn more often
        {
            time = 0;
            timer.WaitTime -= 0.1;
        }

        Random random = new Random();
        int spawn = random.Next(0, Enemies.Count); //Generating random index of which enemy to spawn

        Infantry enemyScene = Enemies[spawn].Instantiate<Infantry>();
        //TODO: enemyScene.AI. 

        enemyScene.GlobalPosition = GetEnemSpawnPosition();

        AddChild(enemyScene);
    }
}
