using Godot;
using Godot.Collections;

/// <summary>
/// Class for implementing enviroment which player encounters
/// </summary>
public partial class Map : Node2D
{
	protected Globals globals; // global variables and functionality
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
		camera = GetNode<Camera2D>("Camera");
		ground = GetNode<TileMap>("TileMapGround");
		playerSpawn = GetNode<Marker2D>("PlayerSpawn");

		// Checking loading state
		switch (globals.LoadingForm)
		{
			case Globals.LoadingForms.Load:
				LoadPlayer(); // loading player from globals
				break;
			case Globals.LoadingForms.Save:
				break; // Doing nothing because loading is handled by Globals
			case Globals.LoadingForms.New:
				SpawnPlayer();
				break;
			default:
				break;
		}
		SetCameraLimits(); // Setting camera limits so the camera won't go beyond borders
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

		globals.Player = player.Save();
		//globals.SaveGame(); // for debuging purposes
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
		globals.Player = player.Save(); // loading player to globals
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

		// Connecting signals
		// TODO: COTR-72 Further code (Connecting signals) might be more readable
		// if moved to GUI.Initialize method
		player.Connect("PlayerDied", new Callable(this, "ShowGameOver"));
		player.Connect("PlayerHealthChanged", new Callable(hud, "ChangeCurrentHealth"));
		player.Connect("PLayerGoldChanged", new Callable(hud, "ChangeCurrency"));
		player.Connect("PlayerMaxHealthChanged", new Callable(hud, "ChangeMaxHealth"));
		player.Connect("PlayerXpChanged", new Callable(hud, "ChangeXP"));
		hud.Initialize(player.Stats);

		player.Load(globals.Player);
		player = GetNode<Player>("Player");
	}

	/// <summary>
	/// Method for parsing Map data to dictionary
	/// </summary>
	/// <returns>Dictionary filled with data to save</returns>
	public virtual Dictionary<string, Variant> Save()
	{
		return new Dictionary<string, Variant>()
		{
			{ "Filename", SceneFilePath },
			{ "Parent", GetParent().GetPath() },
			{ "PosX", Position.X }, // Vector2 is not supported by JSON
			{ "PosY", Position.Y },
			{ "Player", player.Save() }
		};
	}

	/// <summary>
	/// Method for loading Map data from dictionary
	/// </summary>
	/// <param name="data">Dictionary filled with read data</param>
	public virtual void Load(Dictionary<string, Variant> data)
	{
		if (globals.LoadingForm == Globals.LoadingForms.Save)
		{
			LoadSavedPlayer(new Dictionary<string, Variant>((Dictionary<string, Variant>)data["Player"]));
			player = GetNode<Player>("Player");
		}
	}
}
