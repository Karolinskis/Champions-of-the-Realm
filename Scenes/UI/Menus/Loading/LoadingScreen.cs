using Godot;

public partial class LoadingScreen : Control
{
	private Control control;	// control node
	private ProgressBar loadingBar; // loading bar node
	private Godot.Collections.Array loadingBarStatus = new Godot.Collections.Array();	// First element will contain percentage of completion of loading.
	private AnimationPlayer animationPlayer;	// animation player node
	private string nextScene; // stores next scene path

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		control = GetNode<Control>("CanvasLayer/Control/");
		loadingBar = GetNode<ProgressBar>("CanvasLayer/Control/TextureRect/CenterContainer/PanelContainer/VBoxContainer/ProgressBar");
		animationPlayer = GetNode<AnimationPlayer>("CanvasLayer/Control/LoadingAnimation");
	}

	/// <summary>
	/// Called every frame. Used for updating fake loading bar.
	/// </summary>
	/// <param name="value">Loading bar progress.</param>
	public override void _Process(double delta)
	{
		loadingBar.Value += delta*100;
	}

	/// <summary>
	/// Loads a new scene.
	/// </summary>
	/// <param name="scenePath">New scene path</param>
	public void LoadNewScene(string scenePath)
	{
		nextScene = scenePath;
		animationPlayer.Play("TransIn");
	}

	/// <summary>
	/// Loads new scene bit by bit, allowing the use of a loading bar to show progress.
	/// </summary>
	public void InitializeResourceLoader()
	{
		ResourceLoader.ThreadLoadStatus sceneLoadStatus = 0; // gets values from LoadThreadedGetStatus() method.
		ResourceLoader.LoadThreadedRequest(nextScene);	// Begin loading
		while (true)
		{
			// Updates the loading bar progress.
			sceneLoadStatus = ResourceLoader.LoadThreadedGetStatus(nextScene, loadingBarStatus);
			// Loads the new scene once everything has been loaded.
			if (sceneLoadStatus == ResourceLoader.ThreadLoadStatus.Loaded)
			{
				var loadedScene = ResourceLoader.LoadThreadedGet(nextScene) as PackedScene;
            	var newRootNode = loadedScene.Instantiate();
				GetNode("/root").AddChild(newRootNode);
				animationPlayer.Play("TransOut");
				break;
			}
		}
	}

	/// <summary>
	/// Deletes this scene.
	/// </summary>
	public void DeleteScene()
	{
		QueueFree();
	}
}