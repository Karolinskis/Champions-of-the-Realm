using Godot;
using System;

public partial class Weapon : Node2D
{
	[Export] protected float damage { get; set; }
	
	protected Team team;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		team = GetNode<Team>("Team");
	}

	public void Initialize(Team team)
	{
		this.team = team;
	}

	public virtual bool CanAttack() 
	{
		return false;
	}
}