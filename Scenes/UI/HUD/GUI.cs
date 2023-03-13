using Godot;

/// <summary>
/// GUI class that contains graphical UI elements and methods to change them
/// </summary>
public partial class GUI : Control
{
    private ProgressBar healthBar;
    private Label currencyLabel;
    private Label XPLabel;
    private Label currentAmmoLabel;
    private Label maxAmmoLabel;

    /// <summary>
    /// Ready method called when the node enters the scene tree for the first time.
    /// </summary>
    public override void _Ready()
    {
        currencyLabel = GetNode<Label>("HUD/MarginContainer/Rows/TopRow/CurrenctContainer/CurrencyLabel");
        healthBar = GetNode<ProgressBar>("HUD/MarginContainer/Rows/BottomRow/HealthContainer/HealthBar");
        currentAmmoLabel = GetNode<Label>("HUD/MarginContainer/Rows/BottomRow/AmmoContainer/CurrentAmmo");
        XPLabel = GetNode<Label>("HUD/MarginContainer/Rows/TopRow/XPContainer/XPLabel");
        maxAmmoLabel = GetNode<Label>("HUD/MarginContainer/Rows/BottomRow/AmmoContainer/MaxAmmo");
    }

    /// <summary>
    /// Method to initialize player with values
    /// </summary>
    /// <param name="stats">Player stats object</param>
    public void Initialize(Stats stats)
    {
        ChangeCurrentHealth(stats.Health);
        ChangeMaxHealth(stats.MaxHealth);
        ChangeCurrency(stats.Gold);
    }

    /// <summary>
    /// Method to change the current health value
    /// </summary>
    /// <param name="newHealthPoints">New health value</param>
    private void ChangeCurrentHealth(float newHealthPoints)
    {
        healthBar.Value = newHealthPoints;
    }

    /// <summary>
    /// Method to change the max health value
    /// </summary>
    /// <param name="newHealthPoints">New health value</param>
    private void ChangeMaxHealth(float newHealthPoints)
    {
        healthBar.MaxValue = newHealthPoints;
    }

    /// <summary>
    /// Method to change the currency value
    /// </summary>
    /// <param name="newCurrency">New currency value</param>
    private void ChangeCurrency(int newCurrency)
    {
        currencyLabel.Text = "Currency: " + newCurrency;
    }

    /// <summary>
    /// Method to change the XP value
    /// </summary>
    /// <param name="newXP">New XP value</param>
    private void ChangeXP(int newXP)
    {
        XPLabel.Text = "XP: " + newXP;
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
}
