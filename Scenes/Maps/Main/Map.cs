using Godot;
using Godot.Collections;

/// <summary>
/// Class for implementing enviroment
/// </summary>
public partial class Map : Node2D
{
	protected Globals globals;
	protected Player player; // Player in the scene
    protected PackedScene playerScene; // Player resource
    protected PackedScene PauseMenuScene; // Pause menu resource
    protected PackedScene gameOverScene;  // gameOver resource
    protected GUI hud; // GUI in the scene
    protected Camera2D camera; // Player camera
    protected TileMap ground; // Ground level

	protected Marker2D playerSpawn;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
		// Loading resources
		playerScene = ResourceLoader.Load<PackedScene>("res://Scenes/Actors/Player/Player.tscn");
		gameOverScene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Menus/GameOver/GameOver.tscn");
		PauseMenuScene = ResourceLoader.Load<PackedScene>("res://Scenes/UI/Menus/Pause/PauseMenu.tscn");

		// Loading nodes
		globals = GetNode<Globals>("/root/Globals");
		hud = GetNode<GUI>("HUD");
		player = GetNode<Player>("Player");
		camera = GetNode<Camera2D>("Camera");
		ground = GetNode<TileMap>("TileMapGround");
		playerSpawn = GetNode<Marker2D>("PlayerSpawn");
        // Setthing camera
        player.SetCameraTransform(camera.GetPath());
		SetCameraLimits();

		switch (globals.LoadingForm)
		{
			case Globals.LoadingForms.Load:
				break;
			case Globals.LoadingForms.Save:
				break; // Doing nothing because loading is handled by Globals
			case Globals.LoadingForms.New:
				SpawnPlayer();
				break;
			default:
				break;

		}
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
	public void ShowGameOver()
	{
		GameOver gameOver = gameOverScene.Instantiate() as GameOver;
		AddChild(gameOver);
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
	protected void SpawnPlayer()
	{
		player = playerScene.Instantiate<Player>();
		player.Position = playerSpawn.Position;
		AddChild(player);
		player.SetCameraTransform(camera.GetPath());
        player.Connect("PlayerDied", new Callable(this, "ShowGameOver"));

        // Connecting signals
        // TODO: COTR-72 Further code (Connecting signals) might be more readable
		// if moved to GUI.Initialize method
        player.Connect("PlayerHealthChanged", new Callable(hud, "ChangeCurrentHealth"));
        player.Connect("PLayerGoldChanged", new Callable(hud, "ChangeCurrency"));
        player.Connect("PlayerMaxHealthChanged", new Callable(hud, "ChangeMaxHealth"));
        player.Connect("PlayerXpChanged", new Callable(hud, "ChangeXP"));
        hud.Initialize(player.Stats);

		// Alternative: hud.Initialize(player);

		//globals.Player = player.Save();
    }
	/// <summary>
	/// Method for loading saved player
	/// </summary>
	/// <param name="save"></param>
	protected void LoadSavedPlayer(Dictionary<string, Variant> save)
	{
		player = playerScene.Instantiate<Player>();
		player.Position = playerSpawn.Position;
		AddChild(player);
		//player.Load(save)
		player.SetCameraTransform(camera.GetPath());
        player.Connect("PlayerDied", new Callable(this, "ShowGameOver"));

        // Connecting signals
        // TODO: COTR-72 Further code (Connecting signals) might be more readable
		// if moved to GUI.Initialize method
        player.Connect("PlayerHealthChanged", new Callable(hud, "ChangeCurrentHealth"));
        player.Connect("PLayerGoldChanged", new Callable(hud, "ChangeCurrency"));
        player.Connect("PlayerMaxHealthChanged", new Callable(hud, "ChangeMaxHealth"));
        player.Connect("PlayerXpChanged", new Callable(hud, "ChangeXP"));
        hud.Initialize(player.Stats);
		//globals.Player = player.Save();
    }
	/// <summary>
	/// Method for transfering player between scenes/levels
	/// </summary>
	protected void LoadPlayer()
	{
		player = playerScene.Instantiate<Player>();
		player.Position = playerSpawn.Position;
		AddChild(player);
		player.SetCameraTransform(camera.GetPath());
        player.Connect("PlayerDied", new Callable(this, "ShowGameOver"));
		//hud.Initialize(globals.Player);
		//player.Load(globals.Player)
    }
}
