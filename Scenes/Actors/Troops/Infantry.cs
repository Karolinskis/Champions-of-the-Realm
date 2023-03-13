using Godot;
using System;

public partial class Infantry : Troop
{
	public MeleeAI AI { get; set; }
    public override void _Ready()
    {
        base._Ready();
        AI = GetNode<MeleeAI>("MeleeAI");
    }

    public virtual void Attack() { }
    public virtual void RotateAttack() { }
}