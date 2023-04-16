using Godot;
using System;

/// <summary>
/// Class for basic actor implementation 
/// </summary>
public partial class Actor : CharacterBody2D
{
    public Stats Stats { get; set; }  // Property for defining actor stats
    public Team Team { get; set; } // Property for defining actor team
    public Vector2 Direction { get; set; }
    protected CollisionShape2D collisionShape;
    //protected PackedScene bloodScene;
    private Vector2 knockBack = Vector2.Zero;
    public override void _Ready()
    {
        base._Ready();
        Stats = GetNode<Stats>("Stats");
        Team = GetNode<Team>("Team");
        collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
    }
    public override void _PhysicsProcess(double delta)
    {
        knockBack = knockBack.MoveToward(Vector2.Zero, Convert.ToSingle(delta + 10));
        Velocity = Direction * Stats.Speed;
        Velocity += knockBack;
        MoveAndSlide();
    }

    /// <summary>
    /// Method for handling received damage
    /// </summary>
    /// <param name="baseDamage">Amount of damage received</param>
    /// <param name="impactPosition">Position for spawning blood particles</param>
    public virtual void HandleHit(float baseDamage, Vector2 impactPosition)
    {
        float damage = Mathf.Clamp(baseDamage - Stats.Armour, 0, 100);
        Stats.Health -= damage;
        if (Stats.Health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Method for handling death and removing actor from scene
    /// </summary>
    public virtual void Die()
    {
        QueueFree();
    }

    /// <summary>
    /// Method for handling received knockback
    /// </summary>
    /// <param name="amount">Amount of knockback</param>
    /// <param name="impactPosition">Position of knockback received</param>
    public void HandleKnockback(float amount, Vector2 impactPosition)
    {
        Vector2 direction = (impactPosition.DirectionTo(GlobalPosition));
        float strenght = Mathf.Clamp(amount, 5f, 20000f);
        knockBack = direction * strenght;
    }

    /// <summary>
    /// Return the team object of the actor
    /// </summary>
    /// <returns>the team object of the actor</returns>
    public Team.Teams GetTeam()
    {
        return Team.TeamName;
    }

    /// <summary>
    /// Method for getting velocity vector towards certain location
    /// </summary>
    /// <param name="location">Certain global position</param>
    /// <returns></returns>
    public Vector2 VelocityToward(Vector2 location)
    {
        return GlobalPosition.DirectionTo(location) * Stats.Speed;
    }

    /// <summary>
    /// Method for handling actor rotation
    /// </summary>
    /// <param name="location">Location to rotate to</param>
    public void RotateToward(Vector2 location)
    {
        float r = Mathf.LerpAngle(Rotation, GlobalPosition.DirectionTo(location).Angle(), 0.1f);
        if (r < -Math.PI)
        {
            r += (float)Math.PI * 2;
        }
        else if (r > Math.PI)
        {
            r -= (float)Math.PI * 2;
        }
        Rotation = r;
    }
    /// <summary>
    /// Old implementation for handling actor rotation
    /// </summary>
    /// <param name="location">Location to rotate to</param>
    public void RotateTowardLerp(Vector2 location)
    {
        Rotation = Mathf.Lerp(Rotation, GlobalPosition.DirectionTo(location).Angle(), 0.1f);
    }
}
