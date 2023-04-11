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
    [Export] public float TimeStart { get; set; }
    [Export] public float TimeEnd { get; set; }
    [Export] public PackedScene Enemy { get; set; }
    [Export] public int EnemyNum { get; set; }
    [Export] public float EnemySpawnDelay { get; set; }

    public int spawnDelayCounter = 0;
}
