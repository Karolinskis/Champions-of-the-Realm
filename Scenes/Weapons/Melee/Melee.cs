namespace ChampionsOfTheRealm;

public partial class Melee : Weapon
{
    /// <summary>
    /// Keeps track of the cool-down time for the weapon
    /// </summary>
    private Timer cooldownTimer;

    private AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        base._Ready();
        cooldownTimer = GetNode<Timer>("CooldownTimer");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    /// <summary>
    /// Check if the weapon is currently off cool-down and can be used
    /// </summary>
    /// <returns></returns>
    public override bool CanAttack() =>
        cooldownTimer.IsStopped() && !IsAttacking;

    /// <summary>
    /// Define the behavior when the player is not attacking
    /// </summary>
    public override void Idle()
    {
        animationPlayer.Play("Idle");
        IsAttacking = false;
    }

    /// <summary>
    /// Define the behavior when the player initiates an attack
    /// </summary>
    public override void Attack()
    {
        if (CanAttack())
        {
            IsAttacking = true;
            animationPlayer.Play("Attack");
        }
    }
    /// <summary>
    /// Method for placing the weapon on cool-down
    /// </summary>
    public override void Deliver()
    {
        animationPlayer.Play("Idle");
        cooldownTimer.Start();
        IsAttacking = false;
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
            actor.GetTeam() != team)
        {
            actor.HandleHit(damage, GlobalPosition);
            actor.HandleKnockback(knockback, GlobalPosition);
        }
    }

    /// <summary>
    /// Reset isDelivered flag and reenable weapon's collisionShape when cooldownTimer reaches zero.
    /// </summary>
    private void CooldownTimerTimeout()
    {
    }
}
