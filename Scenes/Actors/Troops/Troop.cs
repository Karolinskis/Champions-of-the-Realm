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
    /// Removes the object from the scene tree
    /// </summary>
    public override void Die()
    {
        if (Stats.Gold > 0)
        {
            // Random gold amount is droped
            Random rand = new Random();
            globals.EmitSignal("CoinsDroped", rand.Next(Stats.Gold / 4, Stats.Gold), GlobalPosition);
        }
        EmitSignal(nameof(TroopDied));
        base.Die();
    }
}
