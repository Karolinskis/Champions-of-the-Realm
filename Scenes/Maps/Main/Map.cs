using Godot;
public partial class Map : Node2D
{
    Player player;
    PackedScene playerScene;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        playerScene = ResourceLoader.Load<PackedScene>("res://Scenes/Actors/Player/Player.tscn");
        player = GetNode<Player>("Player");
        player.Connect("Died", new Callable(this, "SpawnPlayer"));
    }

    /// <summary>
    /// Realoading scene after player death
    /// </summary>
    public void SpawnPlayer()
    {
        GetTree().ReloadCurrentScene();
    }
}
