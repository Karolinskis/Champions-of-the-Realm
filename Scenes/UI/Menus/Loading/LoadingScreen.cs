using Godot;
using System;
using System.Threading.Tasks;

public partial class LoadingScreen : Control
{
	private Control control;	// control node
	private ProgressBar loadingBar; // loading bar node
	private Godot.Collections.Array loadingBarStatus = new Godot.Collections.Array();	// First element will contain percentage of completion of loading.
	private AnimationPlayer animationPlayer;	// animation player node
	private TaskCompletionSource<bool> animationFinishedTask;	// stores information on whether animation finished playing.
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		control = GetNode<Control>("CanvasLayer/Control/");
		loadingBar = GetNode<ProgressBar>("CanvasLayer/Control/ColorRect/CenterContainer/PanelContainer/VBoxContainer/ProgressBar");
		animationPlayer = GetNode<AnimationPlayer>("CanvasLayer/Control/LoadingAnimation");
		control.Hide();
	}

	public void UpdateLoadingBar(int value)
	{
		loadingBar.Value = value;
	}

	public async void LoadNewScene(string scenePath)
	{
		control.Show();
		// Loading screen is loaded.
		animationPlayer.Play("TransIn");
		await WaitForAnimationFinished(animationPlayer);

		// New scene is loaded.
		InitializeResourceLoader(scenePath);

		// Loading is done.
		animationPlayer.Play("TransOut");
		await WaitForAnimationFinished(animationPlayer);
		QueueFree();
	}

	/// <summary>
	/// Loads new scene bit by bit, allowing the use of a loading bar to show progress.
	/// </summary>
    /// <param name="scenePath">Path to the next scene</param>
	public void InitializeResourceLoader(string scenePath)
	{
		int sceneLoadStatus = 0; // gets values from LoadThreadedGetStatus() method.
		ResourceLoader.LoadThreadedRequest(scenePath);	// Begin loading
		while (true)
		{
			// Updates the loading bar progress.
			sceneLoadStatus = (int)ResourceLoader.LoadThreadedGetStatus(scenePath, loadingBarStatus);
            UpdateLoadingBar((int)loadingBarStatus[0] * 100);
			// Loads the new scene once everything has been loaded.
			if (sceneLoadStatus == 3)
			{
				var loadedScene = ResourceLoader.LoadThreadedGet(scenePath) as PackedScene;
            	var newRootNode = loadedScene.Instantiate();
				GetNode("/root").AddChild(newRootNode);
				break;
			}
		}
	}

	/// <summary>
	/// Pauses the ChangeScene method until the current animation has finished playing.
	/// </summary>
    /// <param name="animationPlayer">Animation player</param>
    public async Task WaitForAnimationFinished(AnimationPlayer animationPlayer)
    {
        animationFinishedTask = new TaskCompletionSource<bool>();
        await animationFinishedTask.Task;
    }

	/// <summary>
	/// Receiver method for animation_finished signal.
	/// </summary>
    /// <param name="anim_name">Name of the animation that has finished</param>
    public void OnAnimationFinished(string anim_name)
    {
        animationFinishedTask.SetResult(true);
    }
}