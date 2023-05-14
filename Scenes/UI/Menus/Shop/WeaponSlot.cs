namespace ChampionsOfTheRealm;

public partial class WeaponSlot : Panel
{
    [Export] private Weapon weapon;  // Weapon to be bought
    private Player player; // used for getting players gold
    private Label currency; // for displaying currency
    private Label error;

    /// <summary>
    /// Method for initializing weapon, player and currency variables into shop scene.
    /// </summary>
    /// <param name="weapon">Weapon</param>
    /// <param name="player">Player</param>
    /// <param name="currency">Currency label</param>
    public void Initialize(Weapon weapon, Player player, Label currency, Label error)
    {
        // Setting labels to display weapon information
        GetNode<Label>("VBoxContainer/Name").Text = "Melee";    // Weapon name
        GetNode<Button>("VBoxContainer/BuyWeaponButton").Icon = weapon.Icon;    // Buy weapon button
        GetNode<Label>("VBoxContainer/HBoxContainer/DamageValue").Text = weapon.getDamage().ToString();    // Weapon damage stats
        GetNode<Label>("VBoxContainer/HBoxContainer2/KnockbackValue").Text = weapon.getKnockback().ToString("G");   // Weapon knockback stats
        GetNode<Label>("VBoxContainer/HBoxContainer3/CostValue").Text = weapon.getPrice().ToString();   // Weapon cost
        this.weapon = weapon;
        this.player = player;
        this.currency = currency;
        this.error = error;
    }

    /// <summary>
    /// Adds the weapon to players inventory and deletes it from the shop upon button press.
    /// </summary>
    private void BuyWeaponButtonPressed()
    {
        error.Visible = false;
        if (player.Stats.Gold >= weapon.getPrice())
        {
            if (player.WeaponsManager.GetChildCount() == 3)
            {
                error.Text = "Inventory full";
                error.Visible = true;
            }
            else
            {
                int currIndex = player.GetNode<WeaponsManager>("WeaponsManager").GetChildCount();
                player.GetNode<WeaponsManager>("WeaponsManager").AddWeapon(weapon, currIndex);
                player.SetGold(player.Stats.Gold - (int)weapon.getPrice());

                // Updating gold label in shop menu.
                currency.Text = player.Stats.Gold.ToString();
                QueueFree();
            }
        }
        else
        {
            error.Text = "Not enough gold.";
            error.Visible = true;
        }
    }
}