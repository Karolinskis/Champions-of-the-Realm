namespace ChampionsOfTheRealm;

public partial class EnemyMeleeWeapon : Weapon
{
    /// <summary>
    /// Flag inficating whether the weapon has already delivered damage to an object
    /// </summary>
    protected bool isDelivered = false;
    /// <summary>
    /// Keeps track of the cool-down time for the weapon
    /// </summary>
    private Timer cooldownTimer;

    private Timer attackTimer;
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
        attackTimer = GetNode<Timer>("AttackTimer");
    }

    /// <summary>
    /// Check if the weapon is currently off cool-down and can be used
    /// </summary>
    /// <returns></returns>
    public override bool CanAttack()
    {
        return cooldownTimer.IsStopped() && attackTimer.IsStopped();
    }

    /// <summary>
    /// Define the behavior when the player is not attacking
    /// </summary>
    public override void Idle()
    {
    }

    /// <summary>
    /// Define the behavior when the player initiates an attack
    /// </summary>
    public override void Attack()
    {
        if (CanAttack())
        {
            collisionShape.Disabled = false;
            attackTimer.Start();
        }
    }
    // Give the weapon damage to the to the object that was hit
    public override void Deliver()
    {
        collisionShape.Disabled = true;
        cooldownTimer.Start();
    }

    /// <summary>
    /// Define the behavior when the player is moving
    /// </summary>
    public override void Walking()
    {
    }

    /// <summary>
    /// Call when the weapon's collisionShape enters the collision area of another object
    /// </summary>
    /// <param name="body">What body the weapon's collisionShape has entered</param>
    public virtual void Area2dBodyEntered(Node body)
    {
        if (body is Actor actor &&
            actor.GetTeam() != team &&
            !isDelivered)
        {
            actor.HandleHit(damage, GlobalPosition);
            actor.HandleKnockback(knockback, GlobalPosition);
            CallDeferred("Deliver");
            isDelivered = true;
        }
    }

    /// <summary>
    /// Reset isDelivered flag and reenable weapon's collisionShape when cooldownTimer reaches zero.
    /// </summary>
    private void CooldownTimerTimeout()
    {
        isDelivered = false;
    }
    private void AttackTimerTimeout()
    {
        Deliver();
    }
}
