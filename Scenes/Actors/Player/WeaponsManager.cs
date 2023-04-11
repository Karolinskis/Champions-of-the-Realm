using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

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

    /// <summary>
    /// Check if the actor is currently attacking
    /// </summary>
    public bool IsAttacking { get; set; } = false;

    // Team of the actor
    private Team.Teams team;

    // Time of attack
    private Timer attackTimer;

    public override void _Ready()
    {
        CurrentWeapon = GetNode<Weapon>("Melee");
        attackTimer = GetNode<Timer>("Melee/AttackTimer");

        int weaponAmmount = 2;
        weapons = GetChildren()
            .OfType<Weapon>()
            .Take(weaponAmmount)
            .Select(weapon =>
            {
                weapon.Hide();
                return weapon;
            })
            .ToArray();

        CurrentWeapon.Hide();
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
        
        for (int i = 0; i < weapons.Length; i++)
        {
            EmitSignal(nameof(WeaponChanged), i, weapons[i]); 
        }
    }

    /// <summary>
    /// Add weapon to currently carrying weapon list
    /// </summary>
    /// <param name="index">Index to put in to</param>
    public void AddWeapon(Weapon weapon, int index)
    {
        if (weapons[index] != null)
            weapons[index].QueueFree();

        AddChild(weapon);
        weapons[index] = weapon;
        weapon.Hide();

        EmitSignal(nameof(WeaponChanged), index, weapons[index]); 
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
    public void ChangeWeapon()
    {
        int index = System.Array.IndexOf(weapons, CurrentWeapon);
        for (int i = index; i < weapons.Length - 1; i++)
        {
            if (weapons[i + 1] != null)
            {
                SwitchWeapon(weapons[i + 1]);
                return;
            }
        }
        SwitchWeapon(weapons[0]);
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
        CurrentWeapon = weapon;
        EmitSignal(nameof(WeaponChanged), CurrentWeapon);
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
    /// <param name="rotation"></param>
    /// <returns>If can attack</returns>
    public bool Attack(float rotation)
    {
        if (CurrentWeapon != null && CurrentWeapon.CanAttack())
        {
            GD.Print(rotation);
            Rotation = rotation;
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
        IsAttacking = false;
        Player player = GetParent() as Player;
        player.PlayIdle();
        //CurrentWeapon.StartCooldown();
    }

    /// <summary>
    /// Delivers the damage
    /// </summary>
    public void Deliver()
    {
        if (IsAttacking)
        {
            if (CurrentWeapon is Melee melee && attackTimer.IsStopped())
            {
                IsAttacking = false;
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

    public Weapon[] GetWeapons()
    {
        return weapons;
    }

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