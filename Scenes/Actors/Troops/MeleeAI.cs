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
				parent.Velocity = parent.VelocityToward(target.GlobalPosition);
				parent.MoveAndSlide();
				
				break;

			case State.Attack:
				parent.Call("Attack");
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
		// Ignore if the state is already the same
		if(currentState == newState) 
		{
			return;
		}

		currentState = newState;

		if (currentState == State.Idle) 
		{
			parent.Velocity = Vector2.Zero;
			CallDeferred("RefreshDetectionZone");
			return;
		}

		if (currentState == State.Attack)
		{
			parent.Velocity = Vector2.Zero;
			return;
		}	
	}

	/// <summary>
	/// Refresh the detection zone
	/// </summary>
	private void RefreshDetectionZone()
	{
		detectionZone.Monitoring = false;
		detectionZone.Monitoring = true;
	}

	private void DetectionAreaBodyEntered(Actor actor)
	{
		if (parent.GetTeam().TeamName == actor.GetTeam().TeamName && // If the entered actor is in the same team, return
			currentState == State.Attack && currentState == State.Engage) // Check if the actor is currently attacking 
		{
			return;
		}
		
		target = actor;
		ChangeState(State.Engage);
	}

	private void DetectionAreaBodyExited(Actor actor)
	{
		if(actor == target && target != null && !IsQueuedForDeletion())
		{
			target = null;
			ChangeState(State.Idle);
		}	
	}

	private void AttackAreaBodyEntered(Actor actor)
	{
		// If the entered actor is not in the same team, attack
		if(parent.GetTeam().TeamName != actor.GetTeam().TeamName && currentState != State.Attack)
		{
			target = actor;
			ChangeState(State.Attack);
		}
	}
}
