namespace ChampionsOfTheRealm;

/// <summary>
/// Melee weapon class representing a melee wepaon used by the player
/// </summary>
public partial class Melee : Weapon
{
    /// <summary>
    /// Timer for the cool-down time of the weapon
    /// </summary>
    private Timer cooldownTimer;

    /// <summary>
    /// Animation player for playing idle and attacking weapon animations
    /// The majority of weapon functionality is handled by the AnimationPlayer
    /// </summary>
    private AnimationPlayer animationPlayer;

    public override void _Ready()
    {
        base._Ready();
        cooldownTimer = GetNode<Timer>("CooldownTimer");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    /// <summary>
    /// Checks if the weapon is currently off cool-down and can be used
    /// </summary>
    /// <returns>True if the wepaon can be used, false otherwise</returns>
    public override bool CanAttack() =>
        cooldownTimer.IsStopped() && !IsAttacking;

    /// <summary>
    /// Defines the behavior of the weapon when the player is not attacking
    /// </summary>
    public override void Idle()
    {
        animationPlayer.Play("Idle");
        IsAttacking = false;
    }

    /// <summary>
    /// Defines the behavior of the weapon when the player initiates an attack
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
    /// Places the wepaon on cool-down
    /// </summary>
    public override void Deliver()
    {
        animationPlayer.Play("Idle");
        cooldownTimer.Start();
        IsAttacking = false;
    }

    /// <summary>
    /// Defines the behavior of the weapon when the player is walking
    /// </summary>
    public override void Walking()
    {
    }

    /// <summary>
    /// Called when the weapon's collisionShape enters the collision area of another object
    /// </summary>
    /// <param name="body">The body the weapon's collision shape has entered</param>
    public virtual void Area2dBodyEntered(Node body)
    {
        if (body is Actor actor &&
            actor.GetTeam() != team)
        {
            actor.HandleHit(Damage, GlobalPosition);
            actor.HandleKnockback(Knockback, GlobalPosition);
        }
    }

    /// <summary>
    /// Timeout signal is still connected, but the method is no longer used
    /// Method might be used in the future if needed, to implement new functionality
    /// </summary>
    private void CooldownTimerTimeout()
    {
    }
}
