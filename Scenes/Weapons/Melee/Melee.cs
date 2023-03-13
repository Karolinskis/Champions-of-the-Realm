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
	private CollisionShape2D collisionShape;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		collisionShape = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
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
		if (CanAttack())
		{
            collisionShape.Disabled = false;
        }
	}
	// Give the weapon damage to the to the object that was hit
	public virtual void Deliver() 
	{
		collisionShape.Disabled = true;
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
			actor.GetTeam() != team.TeamName &&
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
		collisionShape.Disabled = false;
	}
}
