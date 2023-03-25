using Godot;
using System;

public partial class Infantry : Troop
{
    public MeleeAI AI { get; set; } // MeleeAI for Infantry troops
    protected PackedScene bloodScene; // blood particales
    public override void _Ready()
    {
        base._Ready();
        AI = GetNode<MeleeAI>("MeleeAI");
        bloodScene = ResourceLoader.Load<PackedScene>("res://Material/Particles/Blood/Blood.tscn");
    }
    /// <summary>
    /// Method for Attacking
    /// </summary>
    public virtual void Attack() { }
    /// <summary>
    /// Method for playing Idle animation
    /// </summary>
    public virtual void PlayIdle() { }
    /// <summary>
    /// Method for playing walking animation
    /// </summary>
    public virtual void PlayWalking() { }
    /// <summary>
    /// Method for play attack animation
    /// </summary>
    public virtual void PlayAttack() { }
}