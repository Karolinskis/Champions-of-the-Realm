using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
/// <summary>
/// Class dedicated to implement Player functionality (Missing GUI, Joystick, WeaponManager, DamagePopup)
/// </summary>
public partial class Player : Actor
{
    [Signal]
    public delegate void PlayerHealthChangedEventHandler(float newHealth);
    [Signal]
    public delegate void PLayerGoldChangedEventHandler(int newGold, int oldGold);
    [Signal]
    public delegate void DiedEventHandler();

    [Export] float swingDuration = 0.5f; // TODO swing stab pierce hit
    [Export] float reloadDuration = 1f;
    //private Joystick movementJoystick;
    //private Joystick attackJoystick;
    private AnimationPlayer animationPlayer;
    private RemoteTransform2D cameraTransform;
    private AudioStreamPlayer coinsSound;
    private Vector2 movementDirection = Vector2.Zero;
    private Vector2 attackDirection = Vector2.Zero;
    //private PackedScene damagePopup = (PackedScene)ResourceLoader.Load("res://Scenes/UI/Popups/DamagePopup.tscn");
    //private GUI gui;
    private Globals globals;
    public WeaponsManager WeaponsManager { get; set; }
    public override void _Ready()
	{
        base._Ready();
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        WeaponsManager = GetNode<WeaponsManager>("WeaponsManager");
        //WeaponsManager.Initialize(GetTeam());
        //gui = GetParent().GetNode<GUI>("GUI");
        //movementJoystick = gui.GetNode<Joystick>("MovementJoystick/Joystick_Button");
        //attackJoystick = gui.GetNode<Joystick>("MarginContainer/Rows/MiddleRow/MarginContainer/AttackJoystick/Joystick_Button");
        cameraTransform = GetNode<RemoteTransform2D>("CameraTransform");
        Stats = GetNode<Stats>("Stats");
        globals = GetNode<Globals>("/root/Globals");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
        base._PhysicsProcess(delta);
        direction = Input.GetVector("LEFT", "RIGHT", "UP", "DOWN");
        if (!WeaponsManager.IsAttacking)
        {
            if (movementDirection != Vector2.Zero)
            {
                PlayWalking();
            }
            else
            {
                PlayIdle();
            }
        }
        //attackDirection = attackJoystick.GetValue();
        //movementDirection = movementJoystick.GetValue();

        Velocity = movementDirection * Stats.Speed;

        ////Joystick implementation
        //if (attackJoystick.OngoingDrag != -1)
        //{
        //    LookAt(GlobalPosition + attackDirection);
        //    if (!WeaponsManager.IsAttacking && WeaponsManager.currentWeapon.CanAttack())
        //    {
        //        WeaponsManager.Attack();
        //        PlayAttackAnimation();
        //    }
        //}
        //else if (attackJoystick.OngoingDrag == -1 && WeaponsManager.IsAttacking)
        //{
        //    WeaponsManager.Deliver();
        //}
        //else
        //{
        //    LookAt(GlobalPosition + movementDirection);
        //}
    }
    public override void Die()
    {
        //globals.EmitSignal("CoinsDroped", base.Stats.Gold / 3, GlobalPosition); // DefendMap
        EmitSignal(nameof(Died));
        QueueFree();
    }
    public void SetGold(int gold)
    {
        Stats.Gold = gold;
        EmitSignal(nameof(PLayerGoldChanged), gold, gold);
    }
    public void CancelAttack()
    {
        //WeaponsManager.CancelAttack();
    }
    public void ChangeWeapon()
    {
        //WeaponsManager.ChangeWeapon();
    }
    public void PlayIdle()
    {
        //animationPlayer.Play("Idle");
        //WeaponsManager.Idle();
    }
    public void PlayWalking()
    {
        //animationPlayer.Play("Walking");
        //WeaponsManager.Walking();
    }
    private void PlayAttackAnimation()
    {
        //switch (WeaponsManager.currentWeapon)
        //{
        //    case Fists fists:
        //        animationPlayer.Play("Punsh");
        //        animationPlayer.PlaybackSpeed = Convert.ToSingle(animationPlayer.CurrentAnimationLength / WeaponsManager.currentWeapon.AttackDuartion);
        //        break;
        //    case Bow bow:
        //        animationPlayer.Play("ShootingBow");
        //        animationPlayer.PlaybackSpeed = 1;
        //        break;
        //    case Sword sword:
        //        animationPlayer.Play("SwordSwing");
        //        animationPlayer.PlaybackSpeed = Convert.ToSingle(animationPlayer.CurrentAnimationLength / WeaponsManager.currentWeapon.AttackDuartion);
        //        break;
        //    case Spear spear:
        //        animationPlayer.Play("SpearAttack");
        //        animationPlayer.PlaybackSpeed = Convert.ToSingle(animationPlayer.CurrentAnimationLength / WeaponsManager.currentWeapon.AttackDuartion);
        //        break;
        //}
    }
    public void Walking()
    {
        //if (currentWeapon is Melee melee)
        //{
        //    melee.Walking();
        //}
        //else if (currentWeapon is Bow bow)
        //{
        //    bow.Walking();
        //}
    }
    private void AttackTimerTimeout()
    {
        //if (currentWeapon is Melee)
        //{
        //    Deliver();
        //}
    }
}
