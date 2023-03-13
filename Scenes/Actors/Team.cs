using Godot;
/// <summary>
/// Class dedicated for getting and setting actors team
/// </summary>
public partial class Team : Node
{
    public enum Teams
    {
        NEUTRAL = 0,
        PLAYER = 1,
        ENEMY = 2
    }
    [Export] public Teams TeamName { get; set; } = Teams.NEUTRAL;
}
