using Godot;
using System;

public partial class Infantry : Troop
{
    public MeleeAI AI { get; set; } // MeleeAI for Infantry troops
    public override void _Ready()
    {
        base._Ready();
        AI = GetNode<MeleeAI>("MeleeAI");
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