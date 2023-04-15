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
        AnimateLoadingBar();
    }

    /// <summary>
    /// Animates the fake loading bar.
    /// </summary>
    private void AnimateLoadingBar()
    {
        Tween loadingTween = CreateTween();
        loadingTween.SetTrans(Tween.TransitionType.Expo);
        loadingTween.SetEase(Tween.EaseType.Out);
        loadingTween.TweenMethod(new Callable(this, "ChangeLoadingBarValue"), 0, 100, 4.5f);
    }

    /// <summary>
    /// Changes value of the loading bar.
    /// </summary>
    /// <param name="value">new loading bar value.</param>
    private void ChangeLoadingBarValue(int value)
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
    private void InitializeResourceLoader()
    {
        //LoadProgress();
        ResourceLoader.LoadThreadedRequest(nextScene);  // Begin loading
        ResourceLoader.ThreadLoadStatus sceneLoadStatus = ResourceLoader.LoadThreadedGetStatus(nextScene);
        while (sceneLoadStatus != ResourceLoader.ThreadLoadStatus.Loaded)
        {
            sceneLoadStatus = ResourceLoader.LoadThreadedGetStatus(nextScene);
            // Loads the new scene once everything has been loaded.
            switch (sceneLoadStatus)
            {
                case ResourceLoader.ThreadLoadStatus.InProgress:
                break;

                case ResourceLoader.ThreadLoadStatus.Loaded:
                    PackedScene loadedScene = ResourceLoader.LoadThreadedGet(nextScene) as PackedScene;
                    Node newRootNode = loadedScene.Instantiate();
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
