namespace ChampionsOfTheRealm;

public partial class Troop : Actor
{
    [Signal] public delegate void TroopDiedEventHandler();
    protected Globals globals; // global variables and functionality

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        globals = GetNode<Globals>("/root/Globals");
    }

    /// <summary>
    /// Method for handling received damage
    /// </summary>
    /// <param name="baseDamage">Amount of damage received</param>
    /// <param name="impactPosition">Position for spawning blood particles</param>
    public override void HandleHit(float baseDamage, Vector2 impactPosition)
    {
        float damage = Mathf.Clamp(baseDamage - Stats.Armour, 0, 100);
        Stats.Health -= damage;
        if (Stats.Health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Handling troop death and removing from scene tree
    /// </summary>
    public override void Die()
    {
        if (Stats.Gold > 0)
        {
            // Random gold amount is droped
            Random rand = new Random();
            globals.EmitSignal("CoinsDroped", rand.Next(Stats.Gold / 4, Stats.Gold), GlobalPosition);
        }
        EmitSignal(nameof(TroopDied));
        base.Die();
    }
}
