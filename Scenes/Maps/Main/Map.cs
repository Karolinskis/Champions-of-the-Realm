using Godot;

/// <summary>
/// Class for implementing enviroment
/// </summary>
public partial class Map : Node2D
{
    private Player player; // Player in the scene
    private PackedScene playerScene; // Player resource
    private PackedScene PauseMenuScene; // Pause menu resource
    private GUI hud; // GUI in the scene
    private Camera2D camera; // Player camera
    private TileMap ground; // Ground level

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Loading resources
        playerScene = ResourceLoader.Load<PackedScene>("res://Scenes/Actors/Player/Player.tscn");
        PauseMenuScene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Menus/Pause/PauseMenu.tscn");
        
        // Loading nodes
        hud = GetNode<GUI>("HUD");
        player = GetNode<Player>("Player");
        camera = GetNode<Camera2D>("Camera");
        ground = GetNode<TileMap>("TileMapGround");

        // Setting signals
        player.Connect("PlayerHealthChanged", new Callable(hud, "ChangeCurrentHealth"));
        player.Connect("PLayerGoldChanged", new Callable(hud, "ChangeCurrency"));
        player.Connect("PlayerMaxHealthChanged", new Callable(hud, "ChangeMaxHealth"));
        player.Connect("PlayerDied", new Callable(this, "SpawnPlayer"));
        player.Connect("PlayerXpChanged", new Callable(hud, "ChangeXP"));
        hud.Initialize(player.Stats);

        // Setthing camera
        player.SetCameraTransform(camera.GetPath());
        SetCameraLimits();
    }

    /// <summary>
    /// Pauses this scene
    /// </summary>
    public void Pause()
    {
        PauseMenu pauseMenu = PauseMenuScene.Instantiate() as PauseMenu;
        AddChild(pauseMenu);
    }

    /// <summary>
    /// Realoading scene after player death
    /// </summary>
    public void SpawnPlayer()
    {
        GetTree().ReloadCurrentScene();
    }

    /// <summary>
    /// Method for setting camera limits according to map size
    /// </summary>
    protected void SetCameraLimits()
    {
        Rect2 mapLimits = ground.GetUsedRect();
        Vector2 mapCellSize = ground.TileSet.TileSize;
        // 96px (two tiles of obstacle tilemap)
        camera.LimitLeft = (int)(mapLimits.Position.X * mapCellSize.X) - 96;
        camera.LimitRight = (int)(mapLimits.End.X * mapCellSize.X) - 96;
        camera.LimitTop = (int)(mapLimits.Position.Y * mapCellSize.Y) - 96;
        camera.LimitBottom = (int)(mapLimits.End.Y * mapCellSize.Y) - 96;
    }
}
