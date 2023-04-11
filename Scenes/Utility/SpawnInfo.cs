using Godot;
using System;

/// <summary>
/// Class to contain spawn information
/// </summary>
public partial class SpawnInfo : Resource
{
    /// <summary>
    /// Class parameters
    /// </summary>
    [Export] public float TimeStart { get; set; } //Time when spawning should start
    [Export] public float TimeEnd { get; set; } //Time when spawning should end
    [Export] public PackedScene Enemy { get; set; } //Enemy type to spawn
    [Export] public int EnemyNum { get; set; } //Number of enemies to spawn
    [Export] public float EnemySpawnDelay { get; set; } //Delay between spawns

    public int spawnDelayCounter { get; set; } //Counter for spawn delay
}
