using Godot;
using System;

/// <summary>
/// Class for basic actor implementation 
/// </summary>
public partial class Actor : CharacterBody2D
{
	public Stats Stats { get; set; }
	public Team team { get; set; }
	protected CollisionShape2D collisionShape;
    //protected PackedScene bloodScene;
    protected Vector2 direction;
	private Vector2 knockBack = Vector2.Zero;
	public override void _Ready()
	{
		base._Ready();
		Stats = GetNode<Stats>("Stats");
        team = GetNode<Team>("Team");
        collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        //bloodScene = ResourceLoader.Load<PackedScene>("res://Material/Particles/Impact/Blood.tscn");
    }
	public override void _PhysicsProcess(double delta)
	{
        knockBack = knockBack.MoveToward(Vector2.Zero, Convert.ToSingle(delta + 10));
        Velocity = direction * Stats.Speed;
        Velocity += knockBack;
        MoveAndSlide();
	}

    // Mathod for handeling received damage
    public virtual void HandleHit(float baseDamage, Vector2 impactPosition) { GD.PrintErr("Calling HandleHit from Actor class"); }

    // Method for dealing with actor when health points reaches 0
    public virtual void Die() { GD.PrintErr("Calling Die from Actor class"); }


    //public void HandleKnockback(float amount, Vector2 impactPosition)
    //{
    //    Vector2 direction = (impactPosition.DirectionTo(GlobalPosition));
    //    float strenght = Mathf.Clamp(amount, 5f, 20000f);
    //    knockback = direction * strenght;
    //}

    // For getting velocity towards certain location
    public Vector2 VelocityToward(Vector2 location)
    {
        return GlobalPosition.DirectionTo(location) * Stats.Speed;
    }

    // New implementation of rotation
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
    // Old implementation of rotation
    public void RotateTowardLerp(Vector2 location)
    {
        Rotation = Mathf.Lerp(Rotation, GlobalPosition.DirectionTo(location).Angle(), 0.1f);
    }
}
