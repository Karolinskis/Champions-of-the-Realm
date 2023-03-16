using Godot;
using System;

public partial class Footman : Infantry
{
    private bool isAttacking = false;
    private Timer attackTimer;
    private AnimationPlayer animationPlayer;
    private Melee weapon;
    private Sprite2D sprite; // Actor texture
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        attackTimer = GetNode<Timer>("AttackTimer");
        weapon = GetNode<Melee>("MeleeAI/Melee");
        weapon.Initialize(Team.TeamName);
        sprite = GetNode<Sprite2D>("Sprite2D");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (isAttacking)
        {
            // For animation
            return;
        }

        if (Velocity != Vector2.Zero)
        {
            return;
        }
        weapon.Idle();
    }

    public override void Attack()
    {
        if (!isAttacking && weapon.CanAttack())
        {
            isAttacking = true;
            attackTimer.Start();
            weapon.Attack();
        }
    }

    private void AttackTimerTimeout()
    {
        isAttacking = false;
    }
}
