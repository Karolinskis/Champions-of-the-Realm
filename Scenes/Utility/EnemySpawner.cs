using Godot;
using System;
using System.Collections.Generic;

public partial class EnemySpawner : Node2D
{
	[Export] public Godot.Collections.Array<SpawnInfo> spawns { get; set; }
	private PackedScene enemyScene;
	private int time;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		enemyScene = ResourceLoader.Load<PackedScene>("res://Scenes/Actors/Troops/Infantry/FootmanEnemy.tscn");
		time = 0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnTimerTimeout()
	{
		time++;
		var enemySpawns = spawns;
		foreach(var i in enemySpawns)
		{
			if (time >= i.timeStart && time <= i.timeEnd)
			{
				if (i.spawnDelayCounter < i.enemySpawnDelay) i.spawnDelayCounter++;
				else
				{
                    i.spawnDelayCounter = 0;
					var newEnemy = enemyScene;
					int counter = 0;
					while (counter < i.enemyNum)
					{
						var enemySpawn = newEnemy.Instantiate();
						//reikia nustatyt position (galimai random, bet kol kas preset) enemy spawnui
						//enemySpawn.GlobalPosition = position
						AddChild(enemySpawn);
						counter++;
					}
                }
            }
		}
	}
}
