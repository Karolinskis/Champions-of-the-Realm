using Godot;

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

    //TODO: lacking Joystick scene implementation
    //private Joystick movementJoystick;
    //private Joystick attackJoystick;

    private AnimationPlayer animationPlayer;
    private RemoteTransform2D cameraTransform;
    private AudioStreamPlayer coinsSound;
    private Vector2 movementDirection = Vector2.Zero;
    private Vector2 attackDirection = Vector2.Zero;

    // TODO: lacking damagePopup scene implementation
    //private PackedScene damagePopup = (PackedScene)ResourceLoader.Load("res://Scenes/UI/Popups/DamagePopup.tscn");
    //private GUI gui;

    private Globals globals;
    public WeaponsManager WeaponsManager { get; set; }
    public override void _Ready()
    {
        base._Ready();
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        WeaponsManager = GetNode<WeaponsManager>("WeaponsManager");

        // TODO: lacking weaponsManager, GUI and Joystick implementation
        //WeaponsManager.Initialize(GetTeam());
        //gui = GetParent().GetNode<GUI>("GUI");
        //movementJoystick = gui.GetNode<Joystick>("MovementJoystick/Joystick_Button");
        //attackJoystick = gui.GetNode<Joystick>("MarginContainer/Rows/MiddleRow/MarginContainer/AttackJoystick/Joystick_Button");

        cameraTransform = GetNode<RemoteTransform2D>("CameraTransform");
        Stats = GetNode<Stats>("Stats");
        globals = GetNode<Globals>("/root/Globals");
    }
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
        Velocity = movementDirection * Stats.Speed;

        // TODO: lacking Joystick scene implementation
        //attackDirection = attackJoystick.GetValue();
        //movementDirection = movementJoystick.GetValue();

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
    /// <summary>
    /// Method for handeling received damage
    /// </summary>
    /// <param name="baseDamage">Received damage</param>
    /// <param name="impactPosition">Impact position for calculating impact particales direction</param>
    public override void HandleHit(float baseDamage, Vector2 impactPosition)
    {
        float damage = Mathf.Clamp(baseDamage - Stats.Armour, 0, 100);
        Stats.Health -= damage;
        EmitSignal(nameof(PlayerHealthChanged), Stats.Health);
        GD.Print(Stats.Health);
        if (Stats.Health <= 0)
        {
            Die();
        }

        // TODO: lacks blood and damagePopup scenes implementation
        //else
        //{
        //    Blood blood = bloodScene.Instantiate() as Blood;
        //    GetParent().AddChild(blood);
        //    blood.GlobalPosition = GlobalPosition;
        //    blood.Rotation = impactPosition.DirectionTo(GlobalPosition).Angle();

        //    DamagePopup popup = damagePopup.Instantiate() as DamagePopup;
        //    popup.Amount = (int)damage;
        //    popup.Type = "Damage";
        //    AddChild(popup);
        //}
    }
    /// <summary>
    /// Implemented actor's Die method
    /// </summary>
    public override void Die()
    {
        //globals.EmitSignal("CoinsDroped", base.Stats.Gold / 3, GlobalPosition); // DefendMap
        EmitSignal(nameof(Died));
        QueueFree();
    }

    /// <summary>
    /// Method for handeling received gold and adding to current player amount
    /// </summary>
    /// <param name="newGold">received amount of gold</param>
    public void GetGold(int newGold)
    {
        int oldGold = Stats.Gold;
        Stats.Gold += newGold;
        EmitSignal(nameof(PLayerGoldChanged), oldGold, Stats.Gold);
    }

    /// <summary>
    /// Method for setting player gold 
    /// </summary>
    /// <param name="gold">gold amount to sett</param>
    public void SetGold(int gold)
    {
        Stats.Gold = gold;
        EmitSignal(nameof(PLayerGoldChanged), gold, gold);
    }

    /// <summary>
    /// Method for removing certain amount of gold
    /// </summary>
    /// <param name="removeAmount">Gold amount to remove</param>
    public void RemoveGold(int removeAmount)
    {
        int oldGold = Stats.Gold;
        Stats.Gold -= removeAmount;
        EmitSignal(nameof(PLayerGoldChanged), oldGold, Stats.Gold);
    }

    // TODO: lacks WeaponsManager scene implementation
    /// <summary>
    /// Method for canceling attack
    /// </summary>
    public void CancelAttack()
    {
        //WeaponsManager.CancelAttack();
    }

    /// <summary>
    /// Method for changing weapon
    /// </summary>
    public void ChangeWeapon()
    {
        //WeaponsManager.ChangeWeapon();
    }

    // TODO: lacks WeaponsManager scene implementation and animations
    /// <summary>
    /// Method for playing Idle player animation
    /// </summary>
    public void PlayIdle()
    {
        //animationPlayer.Play("Idle");
        //WeaponsManager.Idle();
    }

    /// <summary>
    /// Method for playing walking player animation
    /// </summary>
    public void PlayWalking()
    {
        //animationPlayer.Play("Walking");
        //WeaponsManager.Walking();
    }

    /// <summary>
    /// Method for playing attack player animation
    /// </summary>
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

    /// <summary>
    /// Method for playing walking animation,
    /// considering equiped weapon
    /// </summary>
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

    /// <summary>
    /// Metheod for handeling attack timer timout
    /// </summary>
    private void AttackTimerTimeout()
    {
        //if (currentWeapon is Melee)
        //{
        //    Deliver();
        //}
    }
}