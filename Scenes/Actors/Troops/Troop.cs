using Godot;

public partial class Troop : Actor
{
    [Signal] public delegate void TroopDiedEventHandler();

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
    }

    /// <summary>
    /// Removes the object from the scene tree
    /// </summary>
    public override void Die()
    {
        if (Stats.Gold > 0)
        {
        }
        EmitSignal(nameof(TroopDied));
        base.Die();
    }
}
