using Godot;
using System;
using System.Reflection.Metadata;

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
            PlayAttacking();
            return;
        }
        if (Velocity != Vector2.Zero)
        {
            PlayWalking();
            return;
        }
        weapon.Idle();
    }
    
    /// <summary>
    /// Method for handling attack
    /// </summary>
    public override void Attack()
    {
        if (!isAttacking && weapon.CanAttack())
        {
            isAttacking = true;
            attackTimer.Start();
            weapon.Attack();
        }
    }

    /// <summary>
    /// Method for playing Idle animation 
    /// Playing idle animation after attacking implemented using animation player track
    /// </summary>
    public override void PlayIdle()
    {
        switch (animationPlayer.CurrentAnimation)
        {
            case "WalkRight":
                animationPlayer.Play("IdleRight");
                break;
            case "WalkBack":
                animationPlayer.Play("IdleRight");
                break;
            case "WalkLeft":
                animationPlayer.Play("IdleLeft");
                break;
            case "WalkFront":
                animationPlayer.Play("IdleFront");
                break;
        }
    }

    /// <summary>
    /// Method for playing walking animation
    /// </summary>
    public override void PlayWalking()
    {
        float angle = Direction.Angle();
        if (angle >= -Math.PI / 4 && angle <= Math.PI / 4)
        {
            animationPlayer.Play("WalkRight");
            return;
        }
        if (angle >= -3 * Math.PI / 4 && angle <= -Math.PI / 4)
        {
            animationPlayer.Play("WalkBack");
            return;
        }
        if (angle >= 3 * Math.PI / 4 || angle <= -3 * Math.PI / 4)
        {
            animationPlayer.Play("WalkLeft");
            return;
        }
        if (angle >= Math.PI / 4 && angle <= 3 * Math.PI / 4)
        {
            animationPlayer.Play("WalkFront");
            return;
        }
    }

    /// <summary>
    /// Method for playing attack animation
    /// </summary>
    public override void PlayAttacking()
    {
        float angle = AI.Rotation;
        if (angle >= -Math.PI / 4 && angle <= Math.PI / 4)
        {
            animationPlayer.Play("AttackRight");
            return;
        }
        if (angle >= -3 * Math.PI / 4 && angle <= -Math.PI / 4)
        {
            animationPlayer.Play("AttackBack");
            return;
        }
        if (angle >= 3 * Math.PI / 4 || angle <= -3 * Math.PI / 4)
        {
            animationPlayer.Play("AttackLeft");
            return;
        }
        if (angle >= Math.PI / 4 && angle <= 3 * Math.PI / 4)
        {
            animationPlayer.Play("AttackFront");
            return;
        }
    }

    /// <summary>
    /// Method for handeling received damage
    /// </summary>
    /// <param name="baseDamage">amount of received damage</param>
    /// <param name="impactPosition">position for calculating particles casting direciton</param>
    public override void HandleHit(float baseDamage, Vector2 impactPosition)
    {
        base.HandleHit(baseDamage, impactPosition);
        Blood blood = bloodScene.Instantiate() as Blood;
        GetParent().AddChild(blood);
        blood.GlobalPosition = GlobalPosition;
        blood.Rotation = impactPosition.DirectionTo(GlobalPosition).Angle();
    }

    /// <summary>
    /// Method for handling attack timer timeout
    /// </summary>
    private void AttackTimerTimeout()
    {
        isAttacking = false;
    }
}
