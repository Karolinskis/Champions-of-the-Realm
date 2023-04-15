using Godot;

public partial class LoadingScreen : Control
{
    private Control control;    // control node
    private ProgressBar loadingBar; // loading bar node
    private AnimationPlayer animationPlayer;    // animation player node
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
    /// <param name="delta">Loading bar progress.</param>
    public override void _Process(double delta)
    {
        loadingBar.Value += delta * 100;
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
        ResourceLoader.LoadThreadedRequest(nextScene);  // Begin loading
        ResourceLoader.ThreadLoadStatus sceneLoadStatus = ResourceLoader.LoadThreadedGetStatus(nextScene);
        while (sceneLoadStatus != ResourceLoader.ThreadLoadStatus.Loaded)
        {
            // Updates the loading bar progress.
            sceneLoadStatus = ResourceLoader.LoadThreadedGetStatus(nextScene);
            // Loads the new scene once everything has been loaded.
            switch (sceneLoadStatus)
            {
                case ResourceLoader.ThreadLoadStatus.InProgress:
                break;

                case ResourceLoader.ThreadLoadStatus.Loaded:
                    var loadedScene = ResourceLoader.LoadThreadedGet(nextScene) as PackedScene;
                    var newRootNode = loadedScene.Instantiate();
                    GetNode("/root").AddChild(newRootNode);
                    animationPlayer.Play("TransOut");
                    break;

                case ResourceLoader.ThreadLoadStatus.Failed:
                    GD.PrintErr("Resource loading failed.");
                    return;

                case ResourceLoader.ThreadLoadStatus.InvalidResource:
                    GD.PrintErr("Cannot load, invalid resource.");
                    return;

                default:
                    GD.PrintErr("State doesn't exist.");
                    return;
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
