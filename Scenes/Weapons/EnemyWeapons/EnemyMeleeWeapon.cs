namespace ChampionsOfTheRealm;

/// <summary>
/// Melee weapon class representing a melee weapon used by an enemy
/// </summary>
public partial class EnemyMeleeWeapon : Weapon
{
    /// <summary>
    /// Flag inficating whether the weapon has already delivered damage to an object
    /// </summary>
    protected bool isDelivered = false;
    /// <summary>
    /// Timer for the cool-down time of the weapon
    /// </summary>
    private Timer cooldownTimer;

    private Timer attackTimer;
    /// <summary>
    /// Collision shape representing the hitbox for the weapon
    /// </summary>
    private CollisionShape2D collisionShape;

    public override void _Ready()
    {
        base._Ready();
        collisionShape = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
        cooldownTimer = GetNode<Timer>("CooldownTimer");
        attackTimer = GetNode<Timer>("AttackTimer");
    }

    /// <summary>
    /// Checks if the weapon is currently off cool-down and can be used.
    /// </summary>
    /// <returns>True if the weapon can be used, false otherwise</returns>
    public override bool CanAttack()
    {
        return cooldownTimer.IsStopped() && attackTimer.IsStopped();
    }

    /// <summary>
    /// Defines the behavior of the weapon when the player is not attacking
    /// </summary>
    public override void Idle()
    {
    }

    /// <summary>
    /// Defines the behavior of the weapon when the player initiates an attack
    /// </summary>
    public override void Attack()
    {
        if (CanAttack())
        {
            collisionShape.Disabled = false;
            attackTimer.Start();
        }
    }

    /// <summary>
    /// Delivers damage to the object that was hit
    /// </summary>
    public override void Deliver()
    {
        collisionShape.Disabled = true;
        cooldownTimer.Start();
    }

    /// <summary>
    /// Defines the behavior of the weapon when the player is moving
    /// </summary>
    public override void Walking()
    {
    }

    /// <summary>
    /// Called when the weapon's collision shape enters the collision area of another object.
    /// </summary>
    /// <param name="body">The body the weapon's collision shape has entered</param>
    public virtual void Area2dBodyEntered(Node body)
    {
        if (body is Actor actor &&
            actor.GetTeam() != team &&
            !isDelivered)
        {
            actor.HandleHit(Damage, GlobalPosition);
            actor.HandleKnockback(Knockback, GlobalPosition);
            CallDeferred("Deliver");
            isDelivered = true;
        }
    }

    /// <summary>
    /// Resets the isDelivered flag and re-enables the weapon's collision shape
    /// </summary>
    private void CooldownTimerTimeout()
    {
        isDelivered = false;
    }

    /// <summary>
    /// Called when the attack timer times out to deliver the attack
    /// </summary>
    private void AttackTimerTimeout()
    {
        Deliver();
    }
}
