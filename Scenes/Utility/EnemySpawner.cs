using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Enemy spawner class
/// </summary>
public partial class EnemySpawner : Node2D
{
	[Export] public Godot.Collections.Array<SpawnInfo> Spawns { get; set; }
	private int time;

	private float limitLeft = 20f;
	private float limitRight;
	private float limitTop = 20f;
	private float limitBottom;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		time = 0;
	}

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
	/// <returns></returns>
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
		foreach(var i in Spawns)
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
