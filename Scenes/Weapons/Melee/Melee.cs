using Godot;
using System;

public partial class Melee : Weapon
{
	/// <summary>
	/// Flag inficating whether the weapon has already delivered damage to an object
	/// </summary>
	protected bool isDelivered = false;
	/// <summary>
	/// Keeps track of the cool-down time for the weapon
	/// </summary>
	private Timer cooldownTimer;
	/// <summary>
	/// Represents the hitbox for the weapon
	/// </summary>
	private Area2D collisionShape;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		collisionShape = GetNode<Area2D>("Area2D");
		cooldownTimer = GetNode<Timer>("CooldownTimer");
	}

	/// <summary>
	/// Check if the weapon is currently off cool-down and can be used
	/// </summary>
	/// <returns></returns>
	public override bool CanAttack()
	{
		return cooldownTimer.IsStopped();
	}

	/// <summary>
	/// Define the behavior when the player is not attacking
	/// </summary>
	public virtual void Idle() 
	{
	}

	/// <summary>
	/// Define the behavior when the player initiates an attack
	/// </summary>
	public virtual void Attack()
	{
	}
	// Give the weapon damage to the to the object that was hit
	public virtual void Deliver() 
	{
		GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Disabled = true;
		StartCooldown();
	}

	/// <summary>
	/// Define the behavior when the player is moving
	/// </summary>
	public virtual void Walking() 
	{
	}

	/// <summary>
	/// Call when the weapon's collisionShape enters the collision area of another object
	/// </summary>
	/// <param name="body">What body the weapon's collisionShape has entered</param>
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

	/// <summary>
	/// Starts the cooldown timer for the weapon
	/// </summary>
	public void StartCooldown() 
	{
		cooldownTimer.Start();
	}

	/// <summary>
	/// Reset isDelivered flag and reenable weapon's collisionShape when cooldownTimer reaches zero.
	/// </summary>
	private void CooldownTimerTimeout()
	{
		isDelivered = false;
		GetNode<CollisionShape2D>("Area2D/CollisionShape2D").Disabled = false;
	}
}
