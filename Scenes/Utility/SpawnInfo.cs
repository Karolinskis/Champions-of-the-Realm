using Godot;
using System;

/// <summary>
/// Class to contain spawn information
/// </summary>
public partial class SpawnInfo : Resource
{
    /// <summary>
    /// Time when spawning should start
    /// </summary>
    /// <value>Time of when spawning starts (float)</value>
    [Export] public float TimeStart { get; set; }
    /// <summary>
    /// Time when spawning should end
    /// </summary>
    /// <value>Time of when spawning ends (float)</value>
    [Export] public float TimeEnd { get; set; }
    /// <summary>
    /// Enemy type to spawn
    /// </summary>
    /// <value>Enemy type to spawn (PackedScene)</value>
    [Export] public PackedScene Enemy { get; set; }
    /// <summary>
    /// Number of enemies to spawn
    /// </summary>
    /// <value>Number of enemies to spawn (int)</value>
    [Export] public int EnemyNum { get; set; }
    /// <summary>
    /// Delay between spawns
    /// </summary>
    /// <value>Time to wait between spawns (float)</value>
    [Export] public float EnemySpawnDelay { get; set; } 

    public int spawnDelayCounter { get; set; } //Counter for spawn delay
}
