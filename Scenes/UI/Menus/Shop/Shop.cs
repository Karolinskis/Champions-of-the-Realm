using Godot;
using System;

public partial class Shop : CanvasLayer
{
    private Globals globals; // global variables and functionality
    private Player player;  // Player character
    private TabBar weaponTab;   // Weapon type tab
    private PackedScene weaponSlot = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Menus/Shop/WeaponSlot.tscn"); // Weapon slot resource
    [Export] private Godot.Collections.Array<PackedScene> weapons; // Weapons array
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        globals = GetNode<Globals>("/root/Globals");
        weaponTab = GetNode<TabBar>("CenterContainer/PanelContainer/MarginContainer/VBoxContainer/TabContainer/Melee"); // loading melee weapons tab
        InitializeWeapons();    // loading weapons into their slots.
    }

    /// <summary>
    /// Method for loading player.
    /// </summary>
    /// <param name="p">Player</param>
    public void Initialize(Player p)
    {
        this.player = p;
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
                slot.Initialize(weapon);
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
        QueueFree();
    }
}
