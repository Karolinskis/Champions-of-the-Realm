using Godot;
using System;

// Class dedicated for storing weapons with switching functionality
public partial class WeaponsManager : Node2D
{
<<<<<<< Updated upstream
    public bool IsAttacking { get; set; } = false; // Is actor attacking
    private Team.Teams team; // Team of the actor
    public Weapon CurrentWeapon { get; set; } // weapon which actor is currently holding
=======
    [Signal]
    public delegate void WeaponChangedEventHandler(Weapon newWeapon);

    /// <summary>
    /// Current weapon the player is holding
    /// </summary>
    /// <value>Weapon object</value>
    public Weapon CurrentWeapon { get; set; }

    /// <summary>
    /// All the weapons the player has
    /// </summary>
    public Weapon[] weapons;

    /// <summary>
    /// Check if the actor is currently attacking
    /// </summary>
    public bool IsAttacking { get; set; } = false;

    // Team of the actor
    private Team.Teams team;

    // Time of attack
    private Timer attackTimer;

    private bool defending = false;

>>>>>>> Stashed changes
    public override void _Ready()
    {
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
    }
    /// <summary>
    /// Method for handeling weapon when idle
    /// </summary>
    public void Idle()
    {
        Rotation = 0;
    }
    /// <summary>
    /// Method for handeling weapon when attacking
    /// </summary>
    /// <param name="rotation"></param>
    /// <returns>If can attack</returns>
    public bool Attack(float rotation)
    {
        if (CurrentWeapon != null && CurrentWeapon.CanAttack())
        {
            Rotation = rotation;
            CurrentWeapon.Attack();
            return true;
        }
        return false;
    }
    /// <summary>
    /// Method for handeling weapon when walking
    /// </summary>
    public void Walking()
    {
    }
<<<<<<< Updated upstream
}
=======

    /// <summary>
    /// If the attack timer runs out it automaticaly delivers damage
    /// </summary>
    private void AttackTimerTimeout()
    {
        if (CurrentWeapon is Melee)
        {
            Deliver();
        }
    }

    private void DefendTimerTimeout()
    {
        defending = false;
    }

    /// <summary>
    /// Save the current weapon loadout
    /// </summary>
    /// <returns>Returns the dictionary of all weapons currently in the loadout</returns>
    public Dictionary<string, Variant> Save()
    {
        Dictionary<string, Variant> data = new Dictionary<string, Variant>();

        for (int i = 0; i < weapons.Length; i++)
        {
            string itemid = "Item" + i;

            if (weapons[i] != null)
            {
                if (weapons[i] is Melee)
                {
                    data.Add(itemid + "Type", "Melee");
                    data.Add(itemid + "Filename", weapons[i].Name);
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
    public void Load(Dictionary<string, Variant> data)
    {
        Weapon setWeapon = new Weapon();

        for (int i = 0; i < weapons.Length; i++)
        {
            string itemid = "Item" + i;

            if ((string)data[itemid + "Type"] == "Melee")
            {
                var newObjectScene = (PackedScene)ResourceLoader.Load(data[itemid + "Filename"].ToString());
                CurrentWeapon = newObjectScene.Instantiate() as Weapon;
                AddWeapon(CurrentWeapon, i);
            }
        }
        Initialize(Team.Teams.PLAYER, CurrentWeapon);
    }
}
>>>>>>> Stashed changes
