using Godot;

/// <summary>
/// Class dedicated for storing actors stats (Health, MaxHealth Armor, Speed, Gold)
/// </summary>
public partial class Stats : Node
{
    [Export] private float _Health;

    public float Health
    {
        // Clamping set Health value, to avoid Health from increasing above MaxHealth or below 0
        get { return _Health; }
        set { _Health = Mathf.Clamp(value, 0, MaxHealth); }
    }
    [Export] public float MaxHealth { get; set; } = 100.0f;
    [Export] public float DamageMultiplier { get; set; } = 1.0f;
    [Export] public float Armour { get; set; } = 0.0f;
    [Export] public float Speed { get; set; } = 300.0f;
    [Export] public int Gold { get; set; } = 0;
}
