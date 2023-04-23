using Godot;
using Godot.Collections;
using System;

/// <summary>
/// Class dedicated to implement Player functionality
/// </summary>
public partial class Player : Actor
{
    [Signal] public delegate void PlayerHealthChangedEventHandler(float newHealth);
    [Signal] public delegate void PLayerGoldChangedEventHandler(int newGold, int oldGold);
    [Signal] public delegate void PlayerMaxHealthChangedEventHandler(int newMaxHealth);
    [Signal] public delegate void PlayerXpChangedEventHandler(float newXp);
    [Signal] public delegate void PlayerDiedEventHandler();

    [Export] float swingDuration = 0.5f; // TODO: swing stab pierce hit
    [Export] float reloadDuration = 1f;

    //TODO: lacking Joystick scene implementation
    //private Joystick movementJoystick;
    //private Joystick attackJoystick;

    /// <summary>
    /// Weapons manager for handeling weapons
    /// </summary>
    public WeaponsManager WeaponsManager { get; set; }

    private AnimationPlayer animationPlayer;

    // Camera transform for setting camera movement according to player movement
    private RemoteTransform2D cameraTransform;
    private AudioStreamPlayer coinsSound; // Sound which is played when currency is received

    private Vector2 movementDirection = Vector2.Zero; // Movement Direction, in which player walks
    private Vector2 attackDirection = Vector2.Zero; // Attack Direction, in which player attacks

    // Level system, whcih handels obtained xp, levelUp and obtaining skills
    private LevelSystem levelSystem;
    private PackedScene bloodScene; // Blood scene for emiting blood particales
    private PackedScene damagePopup; // DamagePopup scene for showing received damage

    private bool canPause = true; // variable for deciding whether pausing is allowed.

    private Globals globals; // Object which handel Global actions (Saving, Loading)

    // Timer for increasing player armour for certain amount of time
    private Timer defendTimer;

    private GpuParticles2D walkingTrail;

    // Audio
    private AudioStreamPlayer2D damagePlayer;
    private AudioStream[] damageSounds;

    public override void _Ready()
    {
        base._Ready();
        // Loading packed scenes
        bloodScene = ResourceLoader.Load<PackedScene>("res://Material/Particles/Blood/Blood.tscn");
        damagePopup = ResourceLoader.Load<PackedScene>("res://Scenes/UI/DamagePopup/DamagePopup.tscn");

        // Load audio streams
        damageSounds = new AudioStream[]
        {
            GD.Load<AudioStream>("res://Sounds/SFX/Characters/Player/Damage/PlayerHurt1.mp3"),
            GD.Load<AudioStream>("res://Sounds/SFX/Characters/Player/Damage/PlayerHurt2.mp3")
        };

        // Getting nodes
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        WeaponsManager = GetNode<WeaponsManager>("WeaponsManager");
        cameraTransform = GetNode<RemoteTransform2D>("CameraTransform");
        Stats = GetNode<Stats>("Stats");
        globals = GetNode<Globals>("/root/Globals");
        defendTimer = GetNode<Timer>("DefendTimer");
        levelSystem = GetNode<LevelSystem>("LevelSystem");
        walkingTrail = GetNode<GpuParticles2D>("WalkingTrail");
        coinsSound = GetNode<AudioStreamPlayer>("CoinsSound");
        damagePlayer = GetNode<AudioStreamPlayer2D>("DamageSoundPlayer");

        // Initializing nodes
        WeaponsManager.Initialize(Team.TeamName, GetNode<Weapon>("WeaponsManager/Melee"));
    }
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        RotateWeapon(); // Rotating weapon according to mouse position

        Direction = Input.GetVector("LEFT", "RIGHT", "UP", "DOWN");
        if (!WeaponsManager.IsAttacking)
        {
            if (Velocity != Vector2.Zero)
            {
                PlayWalking();
                walkingTrail.Emitting = true;
                return;
            }
            else
            {
                PlayIdle();
                walkingTrail.Emitting = false;
            }
        }
        walkingTrail.Emitting = false;
    }

    /// <summary>
    /// Method for handeling Input
    /// </summary>
    /// <param name="event">Input event</param>
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseButton eventMouseButton)
        {
            if (eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.IsPressed() && defendTimer.IsStopped())
            {
                float angle = GetGlobalTransformWithCanvas().Origin.AngleToPoint(eventMouseButton.Position);
                if (WeaponsManager.Attack(angle))
                {
                    PlayAttackAnimation(angle);
                }
            }
            if (eventMouseButton.ButtonIndex == MouseButton.Right && eventMouseButton.IsPressed() && defendTimer.IsStopped())
            {
                Stats.Armour += 10;
                Modulate = new Color("50aaff"); // Defend color
                defendTimer.Start();
            }
        }
        else if (@event is InputEventKey eventKeyboardKey)
        {
            if (eventKeyboardKey.Keycode == Key.Escape && eventKeyboardKey.IsPressed())
            {
                if (canPause)
                {
                    canPause = false;
                    GetParent().Call("Pause");
                }
                else
                {
                    canPause = true;
                }
            }
        }
    }

    /// <summary>
    /// Method for handling received damage
    /// </summary>
    /// <param name="baseDamage">Received damage</param>
    /// <param name="impactPosition">Impact position for calculating impact particles Direction</param>
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
        EmitSignal(nameof(PlayerHealthChanged), Stats.Health);
    }

    /// <summary>
    /// Handling player death
    /// </summary>
    public override void Die()
    {
        EmitSignal(nameof(PlayerDied));
        base.Die();
    }

    /// <summary>
    /// Method for setting new player max health
    /// </summary>
    /// <param name="newMaxHealth">New max health value</param>
    public void SetMaxHealth(int newMaxHealth)
    {
        Stats.MaxHealth = newMaxHealth;
        EmitSignal(nameof(PlayerMaxHealthChanged), Stats.MaxHealth);
    }

    /// <summary>
    /// Method for handeling received gold and adding to current player amount
    /// </summary>
    /// <param name="newGold">received amount of gold</param>
    public void GetGold(int newGold)
    {
        int oldGold = Stats.Gold;
        Stats.Gold += newGold;
        coinsSound.Play();
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

    /// <summary>
    /// Method for handeling received xp
    /// </summary>
    /// <param name="obtainedXp">Received xp</param>
    public void GetXp(float obtainedXp)
    {
        levelSystem.GetXp(obtainedXp);
        EmitSignal(nameof(PlayerXpChanged), levelSystem.CurrrentXp);
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

    /// <summary>
    /// Method for rotating weapons manager according to mouse position
    /// </summary>
    private void RotateWeapon()
    {
        Vector2 mouseCords = GetGlobalMousePosition();
        float angle = GetAngleTo(mouseCords);
        if (angle >= -Math.PI / 2 && angle <= Math.PI / 2)
        {
            WeaponsManager.Scale = new Vector2(Scale.X, 1);
        }
        else
        {
            WeaponsManager.Scale = new Vector2(Scale.X, -1);
        }
        WeaponsManager.Rotation = angle;
    }

    // TODO: lacks WeaponsManager scene implementation and animations
    /// <summary>
    /// Method for playing Idle player animation
    /// </summary>
    public void PlayIdle()
    {
        switch (animationPlayer.CurrentAnimation)
        {
            case ("WalkBack"):
                animationPlayer.Play("IdleBack");
                break;
            case ("WalkFront"):
                animationPlayer.Play("IdleFront");
                break;
            case ("WalkLeft"):
                animationPlayer.Play("IdleLeft");
                break;
            case ("WalkRight"):
                animationPlayer.Play("IdleRight");
                break;
        }
    }

    /// <summary>
    /// Method for playing walking player animation
    /// </summary>
    public void PlayWalking()
    {
        if (Input.IsActionPressed("UP"))
        {
            animationPlayer.Play("WalkBack");
        }
        else if (Input.IsActionPressed("DOWN"))
        {
            animationPlayer.Play("WalkFront");
        }
        else if (Input.IsActionPressed("LEFT"))
        {
            animationPlayer.Play("WalkLeft");
        }
        else if (Input.IsActionPressed("RIGHT"))
        {
            animationPlayer.Play("WalkRight");
        }
        //WeaponsManager.Walking();
    }

    /// <summary>
    /// Method for playing attack animation
    /// </summary>
    private void PlayAttackAnimation(float angle) { }

    /// <summary>
    /// Method for playing walking animation,
    /// considering equiped weapon
    /// </summary>
    public void Walking()
    {
        //if (CurrentWeapon is Melee melee)
        //{
        //    melee.Walking();
        //}
        //else if (CurrentWeapon is Bow bow)
        //{
        //    bow.Walking();
        //}
    }

    /// <summary>
    /// Metheod for handeling attack timer timout
    /// </summary>
    private void AttackTimerTimeout()
    {
        //if (CurrentWeapon is Melee)
        //{
        //    Deliver();
        //}
    }

    /// <summary>
    /// Method for handling defend timer timeout by setting default values
    /// </summary>
    private void DefendTimerTimeout()
    {
        Stats.Armour -= 10;
        Modulate = new Color("ffffff"); // default color
    }

    /// <summary>
    /// Method for connecting camera with player
    /// </summary>
    /// <param name="cameraPath">Path of the camera</param>
    public void SetCameraTransform(NodePath cameraPath)
    {
        cameraTransform.RemotePath = cameraPath;
    }

    /// <summary>
    /// Method for parsing Player data to dictionary
    /// </summary>
    /// <returns>Dictionary filled with data to save</returns>
    public Dictionary<string, Variant> Save()
    {
        return new Dictionary<string, Variant>()
        {
            { "Filename", SceneFilePath },
            { "Parent", GetParent().GetPath() },
            { "PosX", Position.X },
            { "PosY", Position.Y },
            { "Stats.Health", Stats.Health },
            { "Stats.MaxHealth", Stats.MaxHealth },
            { "Stats.DamageMultiplier", Stats.DamageMultiplier },
            { "Stats.Armour", Stats.Armour },
            { "Stats.Speed", Stats.Speed },
            { "Stats.Gold", Stats.Gold },
            { "WeaponsManager", WeaponsManager.Save() },
            { "LevelSystem", levelSystem.Save() }
        };
    }

    /// <summary>
    /// Method for loading Player data from dictionary
    /// </summary>
    /// <param name="data">Dictionary filled with read data</param>
    public virtual void Load(Dictionary<string, Variant> data)
    {
        Position = new Vector2((float)data["PosX"], (float)data["PosY"]);
        Stats.Health = (float)data["Stats.Health"];
        Stats.MaxHealth = (float)data["Stats.MaxHealth"];
        Stats.DamageMultiplier = (float)data["Stats.DamageMultiplier"];
        Stats.Armour = (float)data["Stats.Armour"];
        Stats.Speed = (float)data["Stats.Speed"];
        Stats.Gold = (int)data["Stats.Gold"];
        WeaponsManager.Load(new Dictionary<string, Variant>((Dictionary)data["WeaponsManager"]));
        levelSystem.Load(new Dictionary<string, Variant>((Dictionary)data["LevelSystem"]));
        EmitSignal(nameof(PLayerGoldChanged), Stats.Gold, Stats.Gold);
        EmitSignal(nameof(PlayerHealthChanged), Stats.Health);
        EmitSignal(nameof(PlayerMaxHealthChanged), Stats.MaxHealth);
        EmitSignal(nameof(PlayerXpChanged));
    }
}