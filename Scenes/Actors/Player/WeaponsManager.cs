namespace ChampionsOfTheRealm;

// Class dedicated for storing weapons with switching functionality
public partial class WeaponsManager : Node2D
{
    [Signal] public delegate void WeaponChangedEventHandler(int index, Weapon newWeapon);
    [Signal] public delegate void WeaponSwitchedEventHandler(int index);

    /// <summary>
    /// Current weapon the player is holding
    /// </summary>
    /// <value>Weapon object</value>
    public Weapon CurrentWeapon { get; set; }

    /// <summary>
    /// All the weapons the player has
    /// </summary>
    public Weapon[] weapons;

    // Team of the actor
    private Team.Teams team;

    public override void _Ready()
    {
        int weaponAmmount = 3;
        weapons = GetChildren()
            .OfType<Weapon>()
            .Take(weaponAmmount)
            .Select(weapon =>
            {
                weapon.Hide();
                return weapon;
            })
            .ToArray();
    }

    /// <summary>
    /// Method for initializing required values for weapons manager
    /// </summary>
    /// <param name="setTeam">Weapons team</param>
    /// <param name="weapon">current weapon</param>
    public void Initialize(Team.Teams setTeam, Weapon weapon)
    {
        team = setTeam;
        weapon.Initialize(team);
        CurrentWeapon = weapon;
        CurrentWeapon.Show();

        for (int i = 0; i < weapons.Length; i++)
        {
            EmitSignal(nameof(WeaponChanged), i, weapons[i]);
        }
    }

    /// <summary>
    /// Add weapon to currently carrying weapon list
    /// </summary>
    /// <param name="weapon">Weapon to be added</param>
    /// <param name="index">Index to which weapon should be added to</param>
    /// <returns>True if weapon added succesfully, false otherwise</returns>
    public bool AddWeapon(Weapon weapon, int index)
    {
        if (this.GetChildCount() > 2)
        {
            return false;
        }
        if (index >= weapons.Length)
        {
            Array.Resize(ref weapons, index + 1);
        }
        if (weapons[index] != null)
        {
            weapons[index].QueueFree();
        }
        AddChild(weapon);
        weapons[index] = weapon;
        weapon.Hide();
        EmitSignal(nameof(WeaponChanged), index, weapons[index]);
        return true;
    }

    /// <summary>
    /// Gives the current weapon the player is holding
    /// </summary>
    /// <returns>Returns the weapon object the player is holding</returns>
    public Weapon GetCurrentWeapon()
    {
        return CurrentWeapon;
    }

    /// <summary>
    /// Change the current weapon the user is holding
    /// </summary>
    /// <param name="indexToChangeTo">Index of the weapon to switch to</param>
    public void ChangeWeapon(int indexToChangeTo)
    {
        if (weapons[indexToChangeTo] != null)
        {
            SwitchWeapon(weapons[indexToChangeTo]);
            return;
        }
    }

    /// <summary>
    /// Switch weapon to the given weapon
    /// </summary>
    /// <param name="weapon">Weapon to switch to</param>
    public void SwitchWeapon(Weapon weapon)
    {
        if (CurrentWeapon == weapon) return;
        // Should cancel weapon attack
        CurrentWeapon.Hide();
        weapon.Show();
        weapon.Idle();
        CurrentWeapon = weapon;
    }

    /// <summary>
    /// Method for checking if current weapon is attacking
    /// </summary>
    /// <returns>True if attacking, false if not attacking</returns>
    public bool IsAttacking()
    {
        return CurrentWeapon.IsAttacking;
    }

    /// <summary>
    /// Method for handling weapon when idle
    /// </summary>
    public void Idle()
    {
        Rotation = 0;
        if (CurrentWeapon is Melee melee)
        {
            melee.Idle();
        }
    }

    /// <summary>
    /// Method for handling weapon when attacking
    /// </summary>
    /// <returns>If can attack</returns>
    public bool Attack()
    {
        if (CurrentWeapon != null && CurrentWeapon.CanAttack())
        {
            CurrentWeapon.Attack();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Cancel the current attack
    /// </summary>
    public void CancelAttack()
    {
        CurrentWeapon.Deliver();
    }

    /// <summary>
    /// Delivers the damage
    /// </summary>
    public void Deliver()
    {
        if (IsAttacking())
        {
            if (CurrentWeapon is Melee melee)
            {
                melee.Deliver();
            }
        }
    }

    /// <summary>
    /// Method for handeling weapon when walking
    /// </summary>
    public void Walking()
    {
        if (CurrentWeapon is Melee melee)
        {
            melee.Walking();
        }
    }

    /// <summary>
    /// Get current wepaon array
    /// </summary>
    /// <returns>Returns the current weapons array</returns>
    public Weapon[] GetWeapons()
    {
        return weapons;
    }

    /// <summary>
    /// Save the current weapon loadout
    /// </summary>
    /// <returns>Returns the dictionary of all weapons currently in the loadout</returns>
    public Godot.Collections.Dictionary<string, Variant> Save()
    {
        Godot.Collections.Dictionary<string, Variant> data = new Godot.Collections.Dictionary<string, Variant>();

        for (int i = 0; i < weapons.Length; i++)
        {
            string itemid = "Item" + i;

            if (weapons[i] != null)
            {
                if (weapons[i] is Melee)
                {
                    data.Add(itemid + "Type", "Melee");
                    data.Add(itemid + "Filename", weapons[i].SceneFilePath);
                }
            }
            else
            {
                data.Add(itemid + "Type", "None");
            }
        }

        return data;
    }

    /// <summary>
    /// Load the current weapon loadout
    /// </summary>
    /// <param name="data">Loads all the saved weapons to the loadout</param>
    public void Load(Godot.Collections.Dictionary<string, Variant> data)
    {
        Weapon setWeapon = new Weapon();

        for (int i = 0; i < weapons.Length; i++)
        {
            string itemid = "Item" + i;

            if ((string)data[itemid + "Type"] == "Melee")
            {
                var newObjectScene = ResourceLoader.Load<PackedScene>(data[itemid + "Filename"].ToString());
                CurrentWeapon = newObjectScene.Instantiate<Weapon>();
                AddWeapon(CurrentWeapon, i);
            }
        }
        Initialize(Team.Teams.PLAYER, CurrentWeapon);
    }
}