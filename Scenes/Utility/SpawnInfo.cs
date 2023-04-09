using Godot;
using System;

public partial class SpawnInfo : Resource
{
	[Export] public int timeStart { get; set; }
	[Export] public int timeEnd { get; set; }
    [Export] public Resource enemy { get; set; }
    [Export] public int enemyNum { get; set; }
    [Export] public int enemySpawnDelay { get; set; }

    public int spawnDelayCounter = 0;
}
