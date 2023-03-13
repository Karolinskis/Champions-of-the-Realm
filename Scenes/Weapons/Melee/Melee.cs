using Godot;
using System;

public partial class Melee : Weapon
{
	protected bool isDelivered = false;
	private Timer cooldownTimer;
	private Area2D collisionShape;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		collisionShape = GetNode<Area2D>("Area2D");
		cooldownTimer = GetNode<Timer>("CooldownTimer");
	}

	public override bool CanAttack()
	{
		return cooldownTimer.IsStopped();
	}

	public virtual void Idle() 
	{
	}
	// Start weapon attack
	public virtual void Attack()
	{
	}
	// Give the weapon damage to the to the object that was hit
	public virtual void Deliver() 
	{
		GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Disabled = true;
		StartCooldown();
	}
	public virtual void Walking() 
	{
	}

	public virtual void Area2dBodyEntered(Node body)
	{
		if (body is Actor actor && 
			actor.GetTeam().TeamName != team.TeamName &&
			!isDelivered)
		{
			actor.HandleHit(damage, GlobalPosition);
			CallDeferred("Deliver");
			isDelivered = true;
		}
	}

		// Start the cooldown for attacking
	public void StartCooldown() 
	{
		cooldownTimer.Start();
	}

	// Attack cooldown
	private void CooldownTimerTimeout()
	{
		isDelivered = false;
		GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Disabled = false;
	}
}
