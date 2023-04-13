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
		loadingBar = GetNode<ProgressBar>("CanvasLayer/Control/ColorRect/CenterContainer/PanelContainer/VBoxContainer/ProgressBar");
		animationPlayer = GetNode<AnimationPlayer>("CanvasLayer/Control/LoadingAnimation");
	}

	/// <summary>
	/// Updates loading bar progress.
	/// </summary>
	/// <param name="value">Loading bar progress.</param>
	public void UpdateLoadingBar(int value)
	{
		loadingBar.Value = value;
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
		int sceneLoadStatus = 0; // gets values from LoadThreadedGetStatus() method.
		ResourceLoader.LoadThreadedRequest(nextScene);	// Begin loading
		while (true)
		{
			// Updates the loading bar progress.
			sceneLoadStatus = (int)ResourceLoader.LoadThreadedGetStatus(nextScene, loadingBarStatus);
            UpdateLoadingBar((int)loadingBarStatus[0] * 100);
			// Loads the new scene once everything has been loaded.
			if (sceneLoadStatus == 3)
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