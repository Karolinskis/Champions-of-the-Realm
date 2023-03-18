using Godot;
using System;

// Class dedicated for storing weapons with switching functionality
public partial class WeaponsManager : Node2D
{
    public bool IsAttacking { get; set; } = false; // Is actor attacking
    private Team.Teams team; // Team of the actor
    public Weapon CurrentWeapon { get; set; } // weapon which actor is currently holding
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
            GD.Print(rotation);
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
}
