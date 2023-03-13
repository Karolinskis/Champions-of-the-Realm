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
}