using Godot;
using System;

public partial class Infantry : Actor
{
	public MeleeAI AI { get; set; }

    public override void _Ready()
    {
        base._Ready();
        AI = GetNode<MeleeAI>("MeleeAI");
        //AI.Initialize(this);
    }
}