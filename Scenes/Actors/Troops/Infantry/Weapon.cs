namespace ChampionsOfTheRealm;

public partial class Weapon : Node2D
{
    /// <summary>
    /// Amount of damage weapon can inflict
    /// </summary>
    /// <value>float damage value</value>
    [Export] public float Damage { get; set; }

    /// <summary>
    /// Amount of knockback weapon can inflict
    /// </summary>
    /// <value>float knockback value</value>
    [Export] public float Knockback { get; set; }

    /// <summary>
    /// The price of the weapon in the shop
    /// </summary>
    /// <value>float knockback value</value>
    [Export] public float Price { get; set; }

    /// <summary>
    /// Current Weapon icon
    /// </summary>
    /// <value>Texture2D icon of weapon</value>
    [Export] public Texture2D Icon { get; set; }

    public bool IsAttacking { get; set; } = false;

    protected Team.Teams team;

    public void Initialize(Team.Teams teamName) => team = teamName;

    public virtual bool CanAttack()
    {
        GD.PushError("Calling CanAttack method from weapon class");
        return false;
    }

    public virtual void Idle() =>
        GD.PushError("Calling Idle method from weapon class");

    public virtual void Attack() =>
        GD.PushError("Calling Attack method from weapon class");

    public virtual void Deliver() =>
        GD.PushError("Calling Deliver method from weapon class");

    public virtual void Walking() =>
        GD.PushError("Calling Walking method from weapon class");
}