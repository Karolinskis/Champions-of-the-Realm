using Godot;

public partial class Troop : Actor
{
    [Signal] public delegate void DiedEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
    }

    /// <summary>
    /// Remove health from the troop
    /// </summary>
    /// <param name="baseDamage">Ammount of health to remove</param>
    /// <param name="impactPosition">Position of the hit</param>
    public override void HandleHit(float baseDamage, Vector2 impactPosition)
    {
        base.HandleHit(baseDamage, impactPosition);
        float damage = Mathf.Clamp(baseDamage - Stats.Armour, 0, Stats.MaxHealth);
        Stats.Health -= damage;
        if (Stats.Health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Removes the object from the scene tree
    /// </summary>
    public override void Die()
    {
        base.Die();
        if (Stats.Gold > 0)
        {
        }
        EmitSignal(nameof(Died));
        QueueFree();
    }
}
