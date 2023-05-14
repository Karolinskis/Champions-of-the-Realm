namespace ChampionsOfTheRealm;

public partial class Shop : CanvasLayer
{
    [Export] private Godot.Collections.Array<PackedScene> weapons; // Weapons array
    private Globals globals; // global variables and functionality
    private Player player;  // Player character
    private HBoxContainer weaponTab;   // Weapon type tab
    private Label currencyLabel; // currency label
    private Label errorLabel; // shows errors to the user related to purchasing weapons.
    private PackedScene weaponSlot = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Menus/Shop/WeaponSlot.tscn"); // Weapon slot resource

    public override void _Ready()
    {
        globals = GetNode<Globals>("/root/Globals");
        weaponTab = GetNode<HBoxContainer>("CenterContainer/PanelContainer/MarginContainer/VBoxContainer/TabContainer/Melee/HBoxContainer"); // loading melee weapons tab
        currencyLabel = GetNode<Label>("CenterContainer/PanelContainer/MarginContainer/VBoxContainer/CurrenctContainer/CurrencyLabel"); // loading currency label
        errorLabel = GetNode<Label>("CenterContainer/PanelContainer/MarginContainer/VBoxContainer/ErrorLabel");
        GetTree().Paused = true;
    }

    /// <summary>
    /// Method for loading player.
    /// </summary>
    /// <param name="p">Player</param>
    public void Initialize(Player p)
    {
        this.player = p;
        currencyLabel.Text = player.Stats.Gold.ToString();
    }

    /// <summary>
    /// Loads new weapons into the shop
    /// </summary>
    public void InitializeWeapons()
    {
        foreach (PackedScene weaponScene in weapons)
        {
            if (weaponScene != null)
            {
                Weapon weapon = weaponScene.Instantiate<Weapon>();
                WeaponSlot slot = weaponSlot.Instantiate<WeaponSlot>();
                slot.Initialize(weapon, player, currencyLabel, errorLabel);
                weaponTab.AddChild(slot);
            }
        }
    }

    /// <summary>
    /// Quits shop.
    /// </summary>
    private void ButtonStartPressed()
    {
        // TODO: start new wave
        GetTree().Paused = false;
        QueueFree();
    }
}
