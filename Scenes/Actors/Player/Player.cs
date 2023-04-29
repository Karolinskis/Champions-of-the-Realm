namespace ChampionsOfTheRealm;

using Godot.Collections;

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

    [Export] float SwingDuration  { get; set; } = 0.5f; // TODO: swing stab pierce hit
    [Export] float ReloadDuration { get; set; } = 1f;

    //TODO: lacking Joystick scene implementation
    //private Joystick movementJoystick;
    //private Joystick attackJoystick;

    public WeaponsManager WeaponsManager { get; set; }

    private AnimationPlayer AnimationPlayer { get; set; }

    // Camera transform for setting camera movement according to player movement
    private RemoteTransform2D CameraTransform { get; set; }

    // Sound which is played when currency is received
    private AudioStreamPlayer CoinsSound { get; set; } 

    // Movement Direction, in which player walks
    private Vector2 MovementDirection { get; set; } = Vector2.Zero; 

    // Attack Direction, in which player attacks
    private Vector2 AttackDirection { get; set; } = Vector2.Zero; 

    // Level system, whcih handels obtained xp, levelUp and obtaining skills
    private LevelSystem LevelSystem { get; set; }

    // Blood scene for emiting blood particales
    private PackedScene BloodScene { get; set; }

    // DamagePopup scene for showing received damage
    private PackedScene DamagePopup { get; set; }

    // Variable for deciding whether pausing is allowed
    private bool CanPause { get; set; } = true;

    // Object which handel Global actions (Saving, Loading)
    private Globals Globals { get; set; } 

    // Timer for increasing player armour for certain amount of time
    private Timer DefendTimer { get; set; }

    private GpuParticles2D WalkingTrail { get; set; }

    // Audio
    private AudioStreamPlayer2D DamagePlayer { get; set; }
    private AudioStream[] DamageSounds { get; set; }

    public override void _Ready()
    {
        base._Ready();
        // Loading packed scenes
        BloodScene = ResourceLoader.Load<PackedScene>("res://Material/Particles/Blood/Blood.tscn");
        DamagePopup = ResourceLoader.Load<PackedScene>("res://Scenes/UI/DamagePopup/DamagePopup.tscn");

        // Load audio streams
        DamageSounds = new AudioStream[]
        {
            GD.Load<AudioStream>("res://Sounds/SFX/Characters/Player/Damage/PlayerHurt1.mp3"),
            GD.Load<AudioStream>("res://Sounds/SFX/Characters/Player/Damage/PlayerHurt2.mp3")
        };

        // Getting nodes
        AnimationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        WeaponsManager = GetNode<WeaponsManager>("WeaponsManager");
        CameraTransform = GetNode<RemoteTransform2D>("CameraTransform");
        Stats = GetNode<Stats>("Stats");
        Globals = GetNode<Globals>("/root/Globals");
        DefendTimer = GetNode<Timer>("DefendTimer");
        LevelSystem = GetNode<LevelSystem>("LevelSystem");
        WalkingTrail = GetNode<GpuParticles2D>("WalkingTrail");
        CoinsSound = GetNode<AudioStreamPlayer>("CoinsSound");
        DamagePlayer = GetNode<AudioStreamPlayer2D>("DamageSoundPlayer");

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
                WalkingTrail.Emitting = true;
                return;
            }
            else
            {
                PlayIdle();
                WalkingTrail.Emitting = false;
            }
        }

        WalkingTrail.Emitting = false;
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
            if (eventMouseButton.ButtonIndex == MouseButton.Left && eventMouseButton.IsPressed() && DefendTimer.IsStopped())
            {
                float angle = GetGlobalTransformWithCanvas().Origin.AngleToPoint(eventMouseButton.Position);
                if (WeaponsManager.Attack(angle))
                {
                    PlayAttackAnimation(angle);
                }
            }
            if (eventMouseButton.ButtonIndex == MouseButton.Right && eventMouseButton.IsPressed() && DefendTimer.IsStopped())
            {
                Stats.Armour += 10;
                Modulate = new Color("50aaff"); // Defend color
                DefendTimer.Start();
            }
        }
        else if (@event is InputEventKey eventKeyboardKey)
        {
            if (eventKeyboardKey.Keycode == Key.Escape && eventKeyboardKey.IsPressed())
            {
                if (CanPause)
                {
                    CanPause = false;
                    GetParent().Call("Pause");
                }
                else
                {
                    CanPause = true;
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
        Blood blood = BloodScene.Instantiate() as Blood;
        GetParent().AddChild(blood);
        blood.GlobalPosition = GlobalPosition;
        blood.Rotation = impactPosition.DirectionTo(GlobalPosition).Angle();

        // Showing inflicted damage
        DamagePopup popup = DamagePopup.Instantiate() as DamagePopup;
        popup.Amount = (int)baseDamage;
        popup.Type = "Damage";
        AddChild(popup);

        // Play hit sound
        int damageIndex = new Random().Next(0, DamageSounds.Length);
        DamagePlayer.Stream = DamageSounds[damageIndex];
        DamagePlayer.Play();

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
        CoinsSound.Play();
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
        LevelSystem.GetXp(obtainedXp);
        EmitSignal(nameof(PlayerXpChanged), LevelSystem.CurrrentXp);
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
        switch (AnimationPlayer.CurrentAnimation)
        {
            case ("WalkBack"):
                AnimationPlayer.Play("IdleBack");
                break;
            case ("WalkFront"):
                AnimationPlayer.Play("IdleFront");
                break;
            case ("WalkLeft"):
                AnimationPlayer.Play("IdleLeft");
                break;
            case ("WalkRight"):
                AnimationPlayer.Play("IdleRight");
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
            AnimationPlayer.Play("WalkBack");
        }
        else if (Input.IsActionPressed("DOWN"))
        {
            AnimationPlayer.Play("WalkFront");
        }
        else if (Input.IsActionPressed("LEFT"))
        {
            AnimationPlayer.Play("WalkLeft");
        }
        else if (Input.IsActionPressed("RIGHT"))
        {
            AnimationPlayer.Play("WalkRight");
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
        CameraTransform.RemotePath = cameraPath;
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
            { "LevelSystem", LevelSystem.Save() }
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
        LevelSystem.Load(new Dictionary<string, Variant>((Dictionary)data["LevelSystem"]));
        EmitSignal(nameof(PLayerGoldChanged), Stats.Gold, Stats.Gold);
        EmitSignal(nameof(PlayerHealthChanged), Stats.Health);
        EmitSignal(nameof(PlayerMaxHealthChanged), Stats.MaxHealth);
        EmitSignal(nameof(PlayerXpChanged));
    }
}