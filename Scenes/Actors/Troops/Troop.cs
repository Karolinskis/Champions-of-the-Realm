using Godot;
using System;

public partial class Troop : Actor
{
    [Signal] public delegate void TroopDiedEventHandler();
    protected Globals globals; // global variables and functionality

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        globals = GetNode<Globals>("/root/Globals");
    }

    /// <summary>
    /// Method for handling received damage
    /// </summary>
    /// <param name="baseDamage">amount of received damage</param>
    /// <param name="impactPosition">position for calculating particles casting direciton</param>
    public override void HandleHit(float baseDamage, Vector2 impactPosition)
    {
        if (Stats.Health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Removes the object from the scene tree
    /// </summary>
    public override void Die()
    {
        if (Stats.Gold > 0)
        {
            Random rand = new Random();
            globals.EmitSignal("CoinsDroped", rand.Next(1, Stats.Gold), GlobalPosition);
        }
        EmitSignal(nameof(TroopDied));
        base.Die();
    }
}
