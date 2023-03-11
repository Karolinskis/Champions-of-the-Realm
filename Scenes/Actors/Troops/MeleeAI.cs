using Godot;
using System;

public partial class MeleeAI : Node2D
{

	public enum State
	{
		Idle = 0,	// Waits for next event
		Engage = 1,	// Engages the enemy
		Attack = 2	// Attacks the enemy
	};
	
	protected Area2D detectionZone;
	protected Area2D attackZone;

	private Actor target;
	private Actor parent;
	private State currentState = State.Idle;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		parent = GetParent() as Actor;
		detectionZone = GetNode<Area2D>("DetectionArea");
		attackZone = GetNode<Area2D>("AttackArea");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		switch(currentState) 
		{
			case State.Idle:
				break;

			case State.Engage:
				if(target is null) // Check to see if we actualy have a target
				{
					break;
				}

				parent.RotateToward(target.GlobalPosition);
				GD.Print(target.GlobalPosition);
				parent.Velocity = parent.VelocityToward(target.GlobalPosition);
				parent.MoveAndSlide();
				
				break;

			case State.Attack:
				break;
		}
	}

	// public void Initialize(Actor actor) 
	// {
	// 	parent = actor;
	// }
	
	/// <summary>
	///	Changes the state of the troop
	/// </summary>
	private void ChangeState(State newState)
	{

		if(currentState != newState) 
		{
			currentState = newState;
		}
	}

	private void DetectionAreaBodyEntered(Actor actor)
	{
		GD.Print(actor.GetTeam().TeamName);
		GD.Print("aaa" + parent.GetTeam().TeamName);
		// If the entered actor is in the same team, return
		if (parent.GetTeam().TeamName == actor.GetTeam().TeamName) 
		{
			return;
		}
		
		target = actor;
		ChangeState(State.Engage);
	}
}
