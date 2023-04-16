using Godot;
using System;

public partial class Infantry : Troop
{
    public MeleeAI AI { get; set; } // MeleeAI for Infantry troops
    protected PackedScene bloodScene; // blood particales
    protected PackedScene damagePopup; // For displaying inflicted damage
    public override void _Ready()
    {
        base._Ready();
        AI = GetNode<MeleeAI>("MeleeAI");
        bloodScene = ResourceLoader.Load<PackedScene>("res://Material/Particles/Blood/Blood.tscn");
        damagePopup = ResourceLoader.Load<PackedScene>("res://Scenes/UI/DamagePopup/DamagePopup.tscn");
    }

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

        base.HandleHit(baseDamage, impactPosition);
    }
}