using Godot;
using System;

/// <summary>
/// Class dedicated for storing actors stats (Health, MaxHealth Armor, Speed, Gold)
/// </summary>
public partial class Stats : Node
{
    // [Export] public float Health
    // {
    //     get { return Health; }
    //     set { Health = Mathf.Clamp(value, 0, MaxHealth); }
    // }
    [Export] public float Health { get; set; }
    [Export] public float MaxHealth { get; set; } = 100.0f;
    [Export] public float DamageMultiplier { get; set; } = 1.0f;
    [Export] public float Armour { get; set; } = 0.0f;
    [Export] public float Speed { get; set; } = 300.0f;
    [Export] public int Gold { get; set; } = 0;
}
