using Godot;
using System;

/// <summary>
/// GUI class that contains graphical UI elements and methods to change them
/// </summary>
public partial class GUI : Control
{
    private ProgressBar healthBar;
    private ProgressBar healthBarUnder;
    private Label currencyLabel;
    private Label XPLabel;
    private Label currentAmmoLabel;
    private Label maxAmmoLabel;
    private Tween healthTween;
    private StyleBoxFlat barStyle;
    private Sprite2D goldenCoinSprite;

    private Color healthyColor = new Color("#69a300");
    private Color injuredColor = new Color("ffa300");
    private Color dyingColor = new Color("ff0000");

    private float injuredZone = 0.5f;
    private float dyingZone = 0.2f;

    private Button[] itemArray;
    private int currentItemIndex;

    private Vector2 baseGoldCoinCoordinates; //Saving starting coordinates to reset gold coin position to them after tweening

    Color originalColor = new Color("#690000");
    Color hightlightColor = new Color("#460000");

    /// <summary>
    /// Ready method called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        currencyLabel = GetNode<Label>("HUD/MarginContainer/Rows/TopRow/CurrenctContainer/CurrencyLabel");
        healthBar = GetNode<ProgressBar>("HUD/MarginContainer/Rows/BottomRow/HealthContainer/HealthBar");
        healthBarUnder = GetNode<ProgressBar>("HUD/MarginContainer/Rows/BottomRow/HealthContainer/HealthBar2");
        currentAmmoLabel = GetNode<Label>("HUD/MarginContainer/Rows/BottomRow/AmmoContainer/CurrentAmmo");
        XPLabel = GetNode<Label>("HUD/MarginContainer/Rows/TopRow/XPContainer/XPLabel");
        maxAmmoLabel = GetNode<Label>("HUD/MarginContainer/Rows/BottomRow/AmmoContainer/MaxAmmo");
        barStyle = (StyleBoxFlat)healthBar.Get("theme_override_styles/fill");
        goldenCoinSprite = GetNode<Sprite2D>("HUD/MarginContainer/Rows/TopRow/CurrenctContainer/GoldenCoinSprite");

        baseGoldCoinCoordinates.X = goldenCoinSprite.Position.X;
        baseGoldCoinCoordinates.Y = goldenCoinSprite.Position.Y;

        itemArray = new Button[3];

        itemArray[0] = GetNode<Button>("HUD/MarginContainer/Rows/BottomRow/InventoryContainer/MarginContainer/VBoxContainer/IntentoryItems");
        itemArray[1] = GetNode<Button>("HUD/MarginContainer/Rows/BottomRow/InventoryContainer/MarginContainer/VBoxContainer/IntentoryItems2");
        itemArray[2] = GetNode<Button>("HUD/MarginContainer/Rows/BottomRow/InventoryContainer/MarginContainer/VBoxContainer/IntentoryItems3");
    }

    /// <summary>
    /// Method to initialize player with values
    /// </summary>
    /// <param name="player">Player object</param>
    public void Initialize(Player player)
    {
        // Initializing player stats
        player.Connect("PlayerHealthChanged", new Callable(this, "ChangeCurrentHealth"));
        player.Connect("PLayerGoldChanged", new Callable(this, "ChangeCurrency"));
        player.Connect("PlayerMaxHealthChanged", new Callable(this, "ChangeMaxHealth"));
        player.Connect("PlayerXpChanged", new Callable(this, "ChangeXP"));

        ChangeCurrentHealth(player.Stats.Health);
        ChangeMaxHealth(player.Stats.MaxHealth);
        ChangeCurrency(0, player.Stats.Gold);

        // Initializing hot-bar inventory
        player.WeaponsManager.Connect("WeaponChanged", new Callable(this, "ChangeItem"));
        player.WeaponsManager.Connect("WeaponSwitched", new Callable(this, "SwitchItem"));

        Weapon[] weaponsArray = player.WeaponsManager.GetWeapons();

        for (int i = 0; i < weaponsArray.Length; i++)
        {
            ChangeItem(i, weaponsArray[i]);
        }
    }

    /// <summary>
    /// Method to change the current health value
    /// </summary>
    /// <param name="newHealthPoints">New health value</param>
    private void ChangeCurrentHealth(float newHealthPoints)
    {
        if (newHealthPoints < healthBar.MaxValue * dyingZone) barStyle.BgColor = dyingColor;
        else if (newHealthPoints < healthBar.MaxValue * injuredZone) barStyle.BgColor = injuredColor;
        else barStyle.BgColor = healthyColor;

        healthTween = CreateTween();
        healthTween.SetEase(Tween.EaseType.In);
        healthTween.SetParallel(true);
        healthTween.SetTrans(Tween.TransitionType.Linear);
        healthBar.Value = newHealthPoints;
        healthTween.TweenProperty(healthBarUnder, "value", newHealthPoints, 0.4f).SetDelay(0.1f);
    }

    /// <summary>
    /// Method to change the max health value
    /// </summary>
    /// <param name="newHealthPoints">New health value</param>
    private void ChangeMaxHealth(float newHealthPoints)
    {
        healthBar.MaxValue = newHealthPoints;
        healthBarUnder.MaxValue = newHealthPoints;
    }

    /// <summary>
    /// Method to change the currency value and play specific animations
    /// </summary>
    /// <param name="newCurrency">New currency value</param>
    private void ChangeCurrency(int oldCurrency, int newCurrency)
    {
        goldenCoinSprite.Set("modulate", new Color(1, 1, 1, 1));
        goldenCoinSprite.Set("position", new Vector2(baseGoldCoinCoordinates.X, baseGoldCoinCoordinates.Y));

        //Tweening gold text
        Tween goldTween = CreateTween();
        goldTween.SetTrans(Tween.TransitionType.Expo);
        goldTween.SetEase(Tween.EaseType.Out);
        goldTween.TweenMethod(new Callable(this, "ChangeGoldText"), oldCurrency, newCurrency, 0.6f);

        //Tweening golden coin sprite
        Tween coinTween = CreateTween().SetParallel(true);
        coinTween.SetTrans(Tween.TransitionType.Quart);
        coinTween.TweenProperty(goldenCoinSprite, "position",
            new Vector2(goldenCoinSprite.Position.X, goldenCoinSprite.Position.Y - 10), 1f);
        coinTween.TweenProperty(goldenCoinSprite, "modulate:a", 0, 0.5f).SetDelay(0.5f);
    }

    /// <summary>
    /// Method to change the text of currency label
    /// </summary>
    /// <param name="value"></param>
    private void ChangeGoldText(int value)
    {
        currencyLabel.Text = value.ToString();
    }

    /// <summary>
    /// Method to change the XP value
    /// </summary>
    /// <param name="newXP">New XP value</param>
    private void ChangeXP(int oldXP, int newXP)
    {
        Tween xpTween = CreateTween();
        xpTween.SetTrans(Tween.TransitionType.Expo);
        xpTween.SetEase(Tween.EaseType.Out);
        xpTween.TweenMethod(new Callable(this, "ChangeXPText"), oldXP, newXP, 0.6f);
    }

    /// <summary>
    /// Method to change the text of xp label
    /// </summary>
    /// <param name="value"></param>
    private void ChangeXPText(int value)
    {
        XPLabel.Text = value.ToString();
    }

    /// <summary>
    /// Method to change the current ammo value
    /// </summary>
    /// <param name="newAmmo">New ammo value</param>
    private void ChangeCurrentAmmo(float newAmmo)
    {
        currentAmmoLabel.Text = newAmmo.ToString();
    }

    /// <summary>
    /// Method to change the max ammo value
    /// </summary>
    /// <param name="newAmmo">New ammo value</param>
    private void ChangeMaxAmmo(float newAmmo)
    {
        maxAmmoLabel.Text = newAmmo.ToString();
    }

    /// <summary>
    /// Change current item in index to given weapon
    /// </summary>
    /// <param name="index">Index to change</param>
    /// <param name="weapon">Weapon to change to</param>
    private void ChangeItem(int index, Weapon weapon = null)
    {
        if (weapon is null)
        {
            itemArray[index].Icon = null;
        }

        itemArray[index].Icon = weapon.Icon;
    }

    /// <summary>
    /// Switch item in index
    /// </summary>
    /// <param name="index">Index to switch from</param>
    private void SwitchItem(int index)
    {
        currentItemIndex = index;
    }
}
