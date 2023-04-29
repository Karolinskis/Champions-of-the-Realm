namespace ChampionsOfTheRealm;

using Godot;

public partial class WeaponSlot : Panel
{
    private Weapon weapon;  // Weapon to be bought

    /// <summary>
    /// Initializes all nodes for weapon slot in shop.
    /// </summary>
    public void Initialize(Weapon weapon)
    {
        GetNode<Label>("VBoxContainer/Name").Text = "Melee";    // Weapon name
        GetNode<Button>("VBoxContainer/BuyWeaponButton").Icon = weapon.Icon;    // Buy weapon button
        GetNode<Label>("VBoxContainer/HBoxContainer/DamageValue").Text = "50";    // Weapon damage stats
        GetNode<Label>("VBoxContainer/HBoxContainer2/KnockbackValue").Text = "66";   // Weapon knockback stats
        GetNode<Label>("VBoxContainer/HBoxContainer3/CostValue").Text = "77";   // Weapon cost
        this.weapon = weapon;
    }

    /// <summary>
    /// Deletes the weapon slot when user buys it.
    /// </summary>
    private void BuyWeaponButtonPressed()
    {
        QueueFree();
    }
}