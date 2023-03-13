using Godot;
public partial class Map : Node2D
{
    private Player player;
    private PackedScene playerScene;
    private GUI hud;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Loading resources
        playerScene = ResourceLoader.Load<PackedScene>("res://Scenes/Actors/Player/Player.tscn");

        // Loading nodes
        hud = GetNode<GUI>("HUD");
        player = GetNode<Player>("Player");

        // Setting signals
        player.Connect("PlayerHealthChanged", new Callable(hud, "ChangeCurrentHealth"));
        player.Connect("PLayerGoldChanged", new Callable(hud, "ChangeCurrency"));
        player.Connect("PlayerMaxHealthChanged", new Callable(hud, "ChangeMaxHealth"));
        player.Connect("Died", new Callable(this, "SpawnPlayer"));
        player.Connect("PlayerXpChanged", new Callable(hud, "ChangeXP"));
        hud.Initialize(player.Stats);
    }

    /// <summary>
    /// Realoading scene after player death
    /// </summary>
    public void SpawnPlayer()
    {
        GetTree().ReloadCurrentScene();
    }
}
