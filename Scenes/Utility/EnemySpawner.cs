using Godot;
using System;

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
    [Export] public Godot.Collections.Array<PackedScene> Enemies { get; set; }

    private Timer timer;
    private Player target;

    private int time; //variable to keep count of the time that has passed since spawning started

    private float limitLeft = 20f; //map coordinates limit left
    private float limitRight; //map coordinates limit right
    private float limitTop = 20f; //map coordinates limit top
    private float limitBottom; //map coordinates limit bottom

	/// <summary>
	/// Called when the node enters the scene tree for the first time.
	/// </summary>
	public override void _Ready()
    {
        base._Ready();
        timer = GetNode<Timer>("Timer");
    }

	/// <summary>
	/// Initialize method for initializing limit values
	/// </summary>
	/// <param name="vector">Vector with limit values</param>
	public void Initialize(Vector2 vector, Player player)
    {
        limitRight = vector.X; //Setting right limit of the map
        limitBottom = vector.Y; //Setting bottom limit of the map

        target = player; //Setting target object for the enemies

        timer.Start(); //Starting the timer
    }

    /// <summary>
    /// Method to get a random position for the spawn point
    /// </summary>
    /// <returns>2D vector to reporesent the position of the spawn point</returns>
    private Vector2 GetRandomPosition()
    {
        float x = Globals.GetRandomFloat(limitLeft, limitRight);
        float y = Globals.GetRandomFloat(limitTop, limitBottom);

        return new Vector2(x, y);
    }

    /// <summary>
    /// Method to spawn enemy on timer timeout
    /// </summary>
    private void OnTimerTimeout()
    {
        time++;
        if(time >= 10 && timer.WaitTime > 0.2) //Every 10 seconds the timer wait time is decreased so that enemies spawn more often
        {
            time = 0;
            timer.WaitTime -= 0.1;
        }

        var random = new Random();
        int spawn = random.Next(0, Enemies.Count); //Generating random index of which enemy to spawn

        var enemyScene = Enemies[spawn].Instantiate<Infantry>();
		//TODO: enemyScene.AI. 
		enemyScene.GlobalPosition = GetRandomPosition();
		AddChild(enemyScene);
    }
}
