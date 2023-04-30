using Godot;
using System;

public partial class Infantry : Troop
{
    /// <summary>
    /// MeleeAI for Infantry troops
    /// </summary>
    public MeleeAI AI { get; set; }
    /// <summary>
    /// Blood particles
    /// </summary>
    protected PackedScene bloodScene;
    /// <summary>
    /// For displaying inflicted damage
    /// </summary>
    protected PackedScene damagePopup;

    public override void _Ready()
    {
        base._Ready();
        AI = GetNode<MeleeAI>("MeleeAI");
        bloodScene = ResourceLoader.Load<PackedScene>("res://Material/Particles/Blood/Blood.tscn");
        damagePopup = ResourceLoader.Load<PackedScene>("res://Scenes/UI/DamagePopup/DamagePopup.tscn");
        damagePlayer = GetNode<AudioStreamPlayer2D>("DamageSoundPlayer");

        // Load audio streams
        damageSounds = new AudioStream[]
        {
            GD.Load<AudioStream>("res://Sounds/SFX/Characters/Enemy/Damage/EnemyHurt1.mp3"),
            GD.Load<AudioStream>("res://Sounds/SFX/Characters/Enemy/Damage/EnemyHurt2.mp3"),
            GD.Load<AudioStream>("res://Sounds/SFX/Characters/Enemy/Damage/EnemyHurt3.mp3"),
            GD.Load<AudioStream>("res://Sounds/SFX/Characters/Enemy/Damage/EnemyHurt4.mp3")
        };
    }

    // Audio
    private AudioStreamPlayer2D damagePlayer;
    private AudioStream[] damageSounds;

    /// <summary>
    /// Method for Attacking
    /// </summary>
    public virtual void Attack() { }

    /// <summary>
    /// Method for playing Idle animation
    /// </summary>
    public virtual void PlayIdle() { }

    /// <summary>
    /// Method for playing walking animation
    /// </summary>
    public virtual void PlayWalking() { }

    /// <summary>
    /// Method for playing attacking animation
    /// </summary>
    public virtual void PlayAttacking() { }

    /// <summary>
    /// Method for handling received damage
    /// </summary>
    /// <param name="baseDamage">amount of received damage</param>
    /// <param name="impactPosition">position for calculating particles casting direciton</param>
    public override void HandleHit(float baseDamage, Vector2 impactPosition)
    {
        // Showing blood
        Blood blood = bloodScene.Instantiate() as Blood;
        GetParent().AddChild(blood);
        blood.GlobalPosition = GlobalPosition;
        blood.Rotation = impactPosition.DirectionTo(GlobalPosition).Angle();

        // Showing inflicted damage
        DamagePopup popup = damagePopup.Instantiate() as DamagePopup;
        popup.Amount = (int)baseDamage;
        popup.Type = "Damage";
        AddChild(popup);

        // Play hit sound
        int damageIndex = new Random().Next(0, damageSounds.Length);
        damagePlayer.Stream = damageSounds[damageIndex];
        damagePlayer.Play();

        base.HandleHit(baseDamage, impactPosition);
    }
}