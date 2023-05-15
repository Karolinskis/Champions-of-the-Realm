namespace ChampionsOfTheRealm;

public partial class Shop : CanvasLayer
{
    [Export] private Godot.Collections.Array<PackedScene> weapons; // Weapons array
    private Globals globals; // global variables and functionality
    private Player player;  // Player character
    private TabBar weaponTab;   // Weapon type tab
    private PackedScene weaponSlot = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Menus/Shop/WeaponSlot.tscn"); // Weapon slot resource

    public override void _Ready()
    {
        globals = GetNode<Globals>("/root/Globals");
        weaponTab = GetNode<TabBar>("CenterContainer/PanelContainer/MarginContainer/VBoxContainer/TabContainer/Melee"); // loading melee weapons tab
        InitializeWeapons();    // loading weapons into their slots.
        GetTree().Paused = true;
    }

    /// <summary>
    /// Initializes the shop with the specified player
    /// </summary>
    /// <param name="p">The player character</param>
    public void Initialize(Player p) => this.player = p;

    /// <summary>
    /// Initializes the weapons in the shop by loading them into slots
    /// </summary>
    public void InitializeWeapons()
    {
        foreach (PackedScene weaponScene in weapons)
        {
            if (weaponScene != null)
            {
                Weapon weapon = weaponScene.Instantiate<Weapon>();
                WeaponSlot slot = weaponSlot.Instantiate<WeaponSlot>();
                slot.Initialize(weapon);
                weaponTab.AddChild(slot);
            }
        }
    }

    /// <summary>
    /// Quits the shop and starts a new wave
    /// </summary>
    private void ButtonStartPressed()
    {
        // TODO: start new wave
        GetTree().Paused = false;
        QueueFree();
    }
}
